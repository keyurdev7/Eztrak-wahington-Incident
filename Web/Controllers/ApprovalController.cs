using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Repositories;
using ViewModels.Timesheet;
using ViewModels.CRUD;
using ViewModels.DataTable;
using ViewModels.Shared;
using ViewModels;
using Web.Controllers.Shared;
using Centangle.Common.ResponseHelpers.Models;
using Pagination;
using Newtonsoft.Json;
using Select2;
using Select2.Model;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Web.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class ApprovalController : CrudBaseController<TimesheetModifyViewModel, TimesheetModifyViewModel, TimesheetDetailViewModel, TimesheetDetailViewModel, PayPeriodBriefViewModel, ApprovalSearchViewModel>
    {
        private readonly ITimesheetService _approvalService;
        private readonly ILogger<ApprovalController> _logger;

        public ApprovalController(
            ITimesheetService approvalService
            , ILogger<ApprovalController> logger
            , IMapper mapper)
            : base(approvalService, logger, mapper, "Approval", "Approval")
        {
            _approvalService = approvalService;
            _logger = logger;
        }

        public async Task<ActionResult> ApprovalIndex(long id)
        {
            var workOrderName = await _approvalService.GetWorkOrderName(id);
            var vm = new CrudListViewModel();
            vm.Title = "Approval";
            vm.Filters = new ApprovalSearchViewModel() { WorkOrder = new WorkOrderBriefViewModel { Id = id, Select2Text = workOrderName } };
            vm.DatatableColumns = GetColumns();
            vm.DisableSearch = false;
            vm.HideCreateButton = false;
            vm.ControllerName = "Approval";
            vm.DataUrl = $"/Approval/Search";
            vm.SearchViewPath = $"~/Views/Approval/_Search.cshtml";
            vm = await OverrideCrudListVM(vm);
            return DataTableIndexView(vm);
        }

        public override List<DataTableViewModel> GetColumns()
        {
            return new List<DataTableViewModel>()
            {
                //new DataTableViewModel{title = "<input type='checkbox' id='master-checkbox'>", data = "Id", format = "html", formatValue = "checkbox",className="exclude-form-export" },
                new DataTableViewModel{title = "Status",data = "ApproveStatus",format="html",formatValue="status",orderable = true },
                new DataTableViewModel{title = "Technician",data = "Technician.Name", sortingColumn="Technician.Name", orderable=true},
                new DataTableViewModel{title = "Work Oder",data = "WorkOrder.Name", orderable = true},
                //new DataTableViewModel{title = "Invoice No",data = "InvoiceNo", orderable = true},
                new DataTableViewModel{title = "Week Ending",data = "FormattedWeekEnding", sortingColumn="WeekEnding", orderable = true},
                //new DataTableViewModel{title = "Approve Status",data = "ApproveStatus", orderable = true},
                new DataTableViewModel{title = "Mon",data = "MonSt"},
                new DataTableViewModel{title = "Tue",data = "TueSt"},
                new DataTableViewModel{title = "Wed",data = "WedSt"},
                new DataTableViewModel{title = "Thur",data = "ThurSt"},
                new DataTableViewModel{title = "Fri",data = "FriSt"},
                new DataTableViewModel{title = "Sat",data = "SatSt"},
                new DataTableViewModel{title = "Sun",data = "SunSt"},
                new DataTableViewModel{title = "TOT ST",data = "TOTSt"},
                new DataTableViewModel{title = "TOT OT",data = "TOTOt"},
                new DataTableViewModel{title = "TOT DT",data = "TOTDt"},
                new DataTableViewModel{title = "Total Cost",data = "TotalCostFormatted"},
                //new DataTableViewModel{title = "Total Received Cost",data = "TotalReceivedCostFormatted"},
                //new DataTableViewModel{title = "Balance Due",data = "BalanceDueFormatted"},
                new DataTableViewModel{title = "Action",data = null,className="text-right exclude-form-export"}
            };
        }

        protected override ApprovalSearchViewModel SetDefaultSearchViewModel()
        {
            return new ApprovalSearchViewModel();
        }

        protected override void SetDatatableActions<T>(DatatablePaginatedResultModel<T> result)
        {
            result.ActionsList = new List<DataTableActionViewModel>()
            {
                    new DataTableActionViewModel() {Action="Report",Title="Timesheet",Href=$"#",DatatableHrefType=DatatableHrefType.Link, Class = "timesheet-icon timesheet", Attr=new List<string>{"Id"}, DisableBasedOn = new DisableModel("CanApproveTimesheet","false") },
                    //new DataTableActionViewModel() {Action="Report",Title="Invoice",Href=$"#", Class = "timesheet-icon invoice", Attr=new List<string>{"Id"}},
                    new DataTableActionViewModel() {Action="Delete",Title="Delete",Href = $"/Approval/Delete/Id" }
                };
        }

        public override ActionResult DataTableIndexView(CrudListViewModel vm)
        {
            return View("~/Views/Approval/_Index.cshtml", vm);
        }

        //public override async Task<ActionResult> Index()
        //{
        //    var response = await _assetTypeService.GetAll<AssetTypeDetailViewModel>(new AssetTypeSearchViewModel() { DisablePagination = true });
        //    var assetTypesResponse = response as IRepositoryResponseWithModel<PaginatedResultModel<AssetTypeDetailViewModel>>;
        //    var assetTypes = assetTypesResponse?.ReturnModel.Items ?? new List<AssetTypeDetailViewModel>();
        //    var assetTree = await _assetTypeService.GetHirearchy();
        //    ViewBag.AssetTypes = assetTypes;
        //    ViewBag.AssetTree = assetTree;
        //    return await base.Index();
        //}

        protected override async Task<CrudListViewModel> OverrideCrudListVM(CrudListViewModel vm)
        {
            var html = "";
            html += @"
                    <div class=""col d-flex justify-content-end"" style=""margin-top: -5px;"">
                        <div class=""p-2 d-flex"">
                            <span class=""custom-badge Approved m-1""> </span>
                            <span class=""stat-name"">Approved </span>
                        </div>
                       <div class=""m-2 d-flex"">
                            <span class=""custom-badge UnApproved m-1""> </span>
                            <span class=""stat-name"">Un Approved</span>
                        </div>
                        <div class=""m-2 d-flex"">
                            <span class=""custom-badge Rejected m-1""> </span>
                            <span class=""stat-name"">Rejected</span>
                        </div>
                    </div>
                   ";
            vm.SearchBarHtml = html;
            vm.TitleHtml = @"
            <div class=""d-flex justify-content-start"">
                <h3 class=""page-title text-site-primary m-md-0"">Approval</h3>
            </div>";
            vm.LoadDefaultDatatableScript = true;
            vm.HideCreateButton = true;
            vm.HideTitle = true;
            vm.IsLayoutNull = true;
            vm.LoadDatatableScript = false;
            return vm;
        }

        public async Task<ActionResult> _ApproveTimesheets(List<long> Ids, bool Status)
        {
            try
            {
                await _approvalService.ApproveTimesheets(Ids, Status);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Activity _ApproveTimesheets method threw an exception, Message: {ex.Message}");
                return null;
            }

        }

        public async Task<ActionResult> _GetApprovedTimesheetIds()
        {
            try
            {
                var result = await _approvalService.GetApprovedTimesheetIds();
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Activity _CheckProjectCBOs method threw an exception, Message: {ex.Message}");
                return null;
            }
        }

        [HttpPost]
        public async Task<ActionResult> _TimesheetBreakdown(int id, bool isInvoice)
        {
            var model = await _approvalService.GetTimeSheetBreakdowns(id);
            ViewBag.IsInvoice = isInvoice;
            return View("_TimesheetBreakdown", model);
        }

        [HttpPost]
        public async Task<ActionResult> UpdateBreakdownSheets(TimeSheetWithBreakdownViewModel model)
        {
            if (string.IsNullOrEmpty(model.InvoiceNo))
                model.InvoiceNo = "";
            await _approvalService.UpdateTimeSheetBreakdowns(model);
            var detailModel = (await _approvalService.GetAll<TimesheetDetailViewModel>(new ApprovalSearchViewModel { TimesheetId = model.Id }) as RepositoryResponseWithModel<PaginatedResultModel<TimesheetDetailViewModel>>)?.ReturnModel?.Items ?? new();
            var parsedDetailModel = detailModel.FirstOrDefault();
            return Json(new JsonResultViewModel<TimesheetDetailViewModel> { Success = true, Data = parsedDetailModel });
        }

        public IActionResult ImportExcelSheet()
        {
            var model = new ExcelFileVM();
            return View(model);
        }

        public async Task<JsonResult> PayPeriodSelect2(string prefix, int pageSize, int pageNumber, string customParams)
        {
            try
            {
                var svm = JsonConvert.DeserializeObject<PayPeriodSearchViewModel>(customParams);
                svm.PerPage = pageSize;
                svm.CalculateTotal = true;
                svm.CurrentPage = pageNumber;
                svm.Search = new DataTableSearchViewModel() { value = prefix };
                var items = await _approvalService.GetPayPeriodDates<PayPeriodBriefViewModel>(svm);
                var select2List = GetSelect2List(items);
                return Json(new Select2Repository().GetSelect2PagedResult(pageSize, pageNumber, select2List, items));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Timesheet PayPeriodSelect2 method threw an exception, Message: {ex.Message}");
                return null;
            }
        }

    }
}
