using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositories.Common;
using ViewModels;
using ViewModels.CRUD;
using ViewModels.DataTable;
using ViewModels.Shared.Notes;
using Web.Controllers.Shared;

namespace Web.Controllers
{
    public class StreetServiceRequestController : CrudBaseController<StreetServiceRequestModifyViewModel, StreetServiceRequestModifyViewModel, StreetServiceRequestDetailViewModel, StreetServiceRequestDetailViewModel, StreetServiceRequestBriefViewModel, StreetServiceRequestSearchViewModel>
    {
        private readonly IStreetServiceRequestService<StreetServiceRequestModifyViewModel, StreetServiceRequestModifyViewModel, StreetServiceRequestDetailViewModel> _service;
        private readonly ILogger<StreetServiceRequestController> _logger;

        public StreetServiceRequestController(IStreetServiceRequestService<StreetServiceRequestModifyViewModel, StreetServiceRequestModifyViewModel, StreetServiceRequestDetailViewModel> service, ILogger<StreetServiceRequestController> logger, IMapper mapper) : base(service, logger, mapper, "StreetServiceRequest", "Service Request")
        {
            _service = service;
            _logger = logger;
        }

        public override List<DataTableViewModel> GetColumns()
        {
            return new List<DataTableViewModel>()
            {
                new DataTableViewModel{title = "Status",data = "Status",format="html",formatValue="status", orderable = true },
                new DataTableViewModel{title = "Name",data = "Name", orderable = true},
                new DataTableViewModel{title = "Submitted",data = "FormattedCreatedOn", orderable = true, sortingColumn = "CreatedOn"},
                new DataTableViewModel{title = "Phone",data = "PhoneNumber", orderable = true},
                new DataTableViewModel{title = "Email",data = "Email", orderable = true},
                new DataTableViewModel{title = "Subject",data = "Subject",format="html",formatValue="tooltip", orderable = true},
                new DataTableViewModel{title = "Action",data = null,className="action text-right exclude-form-export"}
            };
        }
        public override ActionResult DataTableIndexView(CrudListViewModel vm)
        {
            return View("_Index", vm);
        }
        protected override void SetDatatableActions<T>(DatatablePaginatedResultModel<T> result)
        {
            result.ActionsList = new List<DataTableActionViewModel>()
            {
                    new DataTableActionViewModel() {Action="CreateWorkOrder",Title="Add",HideBasedOn="HideCreate",Href=$"/WorkOrder/CreateWorkOrderForStreetService/{{Id}}"},
                    new DataTableActionViewModel() {Action="GetNotes",Title="Notes",Class="@HasNotesClass",Href=$"/StreetServiceRequest/GetNotes/{{Id}}"},
                    new DataTableActionViewModel() {Action="Update",Title="Update",Href=$"/StreetServiceRequest/Update/{{Id}}"},
                    new DataTableActionViewModel() {Action="Delete",Title="Delete",Href=$"/StreetServiceRequest/Delete/{{Id}}",DatatableHrefType=DatatableHrefType.Link},
            };
        }
        [Route("/WorkServiceRequest")]
        [AllowAnonymous]
        public async Task<ActionResult> Public()
        {
            var updateVm = GetUpdateViewModel("Create", null);
            updateVm = await OverrideUpdateViewModel(updateVm);
            return View(updateVm);
        }

        [AllowAnonymous]
        public async override Task<ActionResult> Create(StreetServiceRequestModifyViewModel model)
        {
            if (!ModelState.IsValid && !User.Identity.IsAuthenticated)
            {
                return View("~/Views/StreetServiceRequest/Update.cshtml", model);
            }
            model.Status = Enums.SSRStatus.Open;
            return await base.Create(model);
        }
        [AllowAnonymous]
        [Route("/WorkServiceRequest/Success")]
        public ActionResult UnAuthorizedSuccess()
        {
            return View("UnAuthorizedSuccessResponse");
        }

        public override ActionResult OnSuccessReturnMethod()
        {
            return RedirectToAction("Index");
        }

        protected override async Task<CrudListViewModel> OverrideCrudListVM(CrudListViewModel vm)
        {
            var html = "";
            html += @"
                    <div class=""col d-flex justify-content-end"" style=""margin-top: -5px;"">
                        <div class=""p-2 d-flex"">
                            <span class=""custom-badge Dismissed m-1""> </span>
                            <span class=""stat-name"">Dismissed</span>
                        </div>
                        <div class=""m-2 d-flex"">
                            <span class=""custom-badge WOCreated m-1""> </span>
                            <span class=""stat-name"">WO Created</span>
                        </div>
                       <div class=""m-2 d-flex"">
                            <span class=""custom-badge Open m-1""> </span>
                            <span class=""stat-name"">Open</span>
                        </div>
                    </div>
                   ";
            vm.SearchBarHtml = html;
            vm.HideTitle = true;
            vm.IsLayoutNull = true;
            vm.LoadDatatableScript = false;
            return vm;
        }
        [AllowAnonymous]
        public async Task<ActionResult> CreateStreetServiceRequest()
        {
            var model = new StreetServiceRequestModifyViewModel();
            return View("~/Views/StreetServiceRequest/Update.cshtml", model);
        }
        public async Task<ActionResult> GetNotes(int id)
        {
            try
            {
                var notes = await _service.GetNotesBySSRId(id);
                List<INotesViewModel> notesViewModels = notes.Cast<INotesViewModel>().ToList();
                ViewBag.StreetServiceRequestId = id;
                return View("_Notes", notesViewModels);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Street Service Request Notes method threw an exception, Message: {ex.Message}");
                return RedirectToAction("Index");
            }
        }



        public async Task<ActionResult> SaveNotes(SSRNotesViewModel model)
        {
            var notes = await _service.SaveNotes(model);
            return Json(notes);
        }
    }
}

