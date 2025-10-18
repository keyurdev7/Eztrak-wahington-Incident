using AutoMapper;
using Centangle.Common.ResponseHelpers.Models;
using DataLibrary;
using Microsoft.AspNetCore.Mvc;
using Repositories.Common;
using ViewModels;
using ViewModels.CRUD;
using ViewModels.DataTable;
using ViewModels.Shared.Notes;
using Web.Controllers.Shared;

namespace Web.Controllers
{
    public class TicketController : CrudBaseController<TicketModifyViewModel, TicketModifyViewModel, TicketDetailViewModel, TicketDetailViewModel, TicketBriefViewModel, TicketSearchViewModel>
    {
        private readonly ITicketService<TicketModifyViewModel, TicketModifyViewModel, TicketDetailViewModel> _service;
        private readonly ILogger<TicketController> _logger;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _db;
        public TicketController(ApplicationDbContext db, ITicketService<TicketModifyViewModel, TicketModifyViewModel, TicketDetailViewModel> service, ILogger<TicketController> logger, IMapper mapper) : base(service, logger, mapper, "Ticket", "Ticket")
        {
            _service = service;
            _logger = logger;
            _mapper = mapper;
            _db = db;
        }


        public override List<DataTableViewModel> GetColumns()
        {
            return new List<DataTableViewModel>()
            {
                new DataTableViewModel{title = "Status",data = "Status",  format = "html",formatValue = "status"},
                new DataTableViewModel{title = "Ticket No.",data = "TicketNo"},
                new DataTableViewModel{title = "Category",data = "FormattedCategory"},
                new DataTableViewModel{title = "Description",data = "Description"},
                new DataTableViewModel{title = "IT Response",data = "ITResponse"},
                new DataTableViewModel{title = "Date Submitted",data = "FormattedCreatedOn"},
                new DataTableViewModel{title = "Submitted By",data = "CreatedBy"},
                new DataTableViewModel{title = "Updated By",data = "UpdatedBy"},
                new DataTableViewModel{title = "Action",data = null,className="action text-right exclude-form-export"}

            };
        }
        protected override async void SetDatatableActions<T>(DatatablePaginatedResultModel<T> result)
        {
            result.ActionsList = new List<DataTableActionViewModel>()
            {
                new DataTableActionViewModel() {Action="UpdateStatus",Title="View",Href=$"/Ticket/UpdateTicketStatusView/{{Id}}"},
                new DataTableActionViewModel() {Action="GetTicketHistory",Title="History",Href=$"/Ticket/GetTicketHistory/{{Id}}"},
            };
        }
        protected override async Task<CrudListViewModel> OverrideCrudListVM(CrudListViewModel vm)
        {
            var html = "";
            html += @"
                    <div class=""col-12 d-flex justify-content-end"">
                        <div class=""p-2 d-flex"">
                            <span class=""custom-badge Good m-1""> </span>
                            <span class=""stat-name"">Completed</span>
                        </div>
                        <div class=""m-2 d-flex"">
                            <span class=""custom-badge Fair m-1""> </span>
                            <span class=""stat-name"">Pending</span>
                        </div>
                        <div class=""m-2 d-flex"">
                            <span class=""custom-badge Poor m-1""> </span>
                            <span class=""stat-name"">Open</span>
                        </div>
                        <div class=""m-2 d-flex"">
                            <span class=""custom-badge OutOfService m-1""> </span>
                            <span class=""stat-name"">Canceled</span>
                        </div>
                    </div>
                   ";
            vm.SearchBarHtml = html;
            vm.TitleHtml = @"
            <div class=""d-flex justify-content-start"">
                <h3 class=""page-title text-site-primary m-md-0"">Ticket</h3>
                
            </div>";
            return vm;
        }

        [HttpGet]
        public async Task<IActionResult> UpdateTicketStatusView(int id)
        {
            var ticket = await _service.GetById(id);
            var parsedModel = ticket as RepositoryResponseWithModel<TicketDetailViewModel>;
            var viewModel = _mapper.Map<TicketStatusUpdateViewModel>(parsedModel.ReturnModel);
            return View("_UpdateTicketStatus", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTicketStatus(TicketStatusUpdateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _service.UpdateTicket(model);
                if (result)
                {
                    return Json(new { status = result });
                }
            }
            var invalidFieldErrors = ModelState.Where(x => x.Value.Errors.Any())
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
            );
            return Json(new { status = false, fieldErrors = invalidFieldErrors });
        }

        public async Task<IActionResult> GetTicketHistory(int id)
        {
            var ticketHistories = await _service.GetTicketHistory(id);
            return View("_TicketHistory", ticketHistories);
        }

        public async Task<ActionResult> _GetAttachmentView(long Id)
        {
            try
            {
                var attachments = await _service.GetTicketAttachments(Id);
                return View("~/Views/Shared/GalleryPartialView/_GetGalleryView.cshtml", attachments);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Asset _GetGalleryView method threw an exception, Message: {ex.Message}");
                return null;
            }

        }

        public async Task<string> _GetAttachmentUrl(long Id)
        {
            try
            {
                var attachmentUrl = await _service.GetAttachmentUrl(Id);
                return attachmentUrl;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Asset _GetAttachmentUrl method threw an exception, Message: {ex.Message}");
                return null;
            }
        }
    }
}

