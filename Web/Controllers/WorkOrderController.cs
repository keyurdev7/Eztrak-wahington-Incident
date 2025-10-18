using AutoMapper;
using Repositories.Common;
using ViewModels.DataTable;
using ViewModels;
using Web.Controllers.Shared;
using Microsoft.AspNetCore.Mvc;
using Centangle.Common.ResponseHelpers.Models;
using Enums;
using ViewModels.CRUD;
using ViewModels.Shared.Notes;
using ViewModels.WorkOrderTechnician;
using ViewModels.WorkOrderTasks;
using Repositories.Shared.UserInfoServices.Interface;
using System.Net;
using DocumentFormat.OpenXml.Office2010.Excel;
using Repositories.Services.Dashboard.Interface;
using Web.Helpers;
using Pagination;

namespace Web.Controllers
{
    public class WorkOrderController : CrudBaseController<WorkOrderModifyViewModel, WorkOrderModifyViewModel, WorkOrderDetailViewModel, WorkOrderDetailViewModel, WorkOrderDetailViewModel, WorkOrderSearchViewModel>
    {
        private readonly IWorkOrderService<WorkOrderModifyViewModel, WorkOrderModifyViewModel, WorkOrderDetailViewModel> _service;
        private readonly IAssetService<AssetModifyViewModel, AssetModifyViewModel, AssetDetailViewModel> _assetService;
        private readonly ILogger<WorkOrderController> _logger;
        private readonly IDashboardService _dashboardService;
        private readonly IMapper _mapper;
        private readonly IUserInfoService _userInfoService;
        private readonly IUserSearchSettingService<UserSearchSettingModifyViewModel, UserSearchSettingModifyViewModel, UserSearchSettingDetailViewModel> _userSearchSettingService;
        private readonly IDynamicColumnService<DynamicColumnModifyViewModel, DynamicColumnModifyViewModel, DynamicColumnOptionDetailViewModel> _dynamicColumnService;

        public WorkOrderController(
            IWorkOrderService<WorkOrderModifyViewModel, WorkOrderModifyViewModel, WorkOrderDetailViewModel> service
            , IAssetService<AssetModifyViewModel, AssetModifyViewModel, AssetDetailViewModel> assetService
            , ILogger<WorkOrderController> logger
            , IDashboardService dashboardService
            , IMapper mapper
            , IUserInfoService userInfoService
            , IUserSearchSettingService<UserSearchSettingModifyViewModel, UserSearchSettingModifyViewModel, UserSearchSettingDetailViewModel> userSearchSettingService
            , IDynamicColumnService<DynamicColumnModifyViewModel, DynamicColumnModifyViewModel, DynamicColumnOptionDetailViewModel> dynamicColumnService) : base(service, logger, mapper, "WorkOrder", "Work Orders", true, false, false)
        {
            _service = service;
            _assetService = assetService;
            _logger = logger;
            _dashboardService = dashboardService;
            _mapper = mapper;
            _userInfoService = userInfoService;
            _userSearchSettingService = userSearchSettingService;
            _dynamicColumnService = dynamicColumnService;
        }

        protected override WorkOrderSearchViewModel SetDefaultSearchViewModel()
        {
            var model = base.SetDefaultSearchViewModel();
            if (Request.QueryString.HasValue)
            {
                var assetSystemGeneratedId = HttpContext.Request.Query["assetId"].ToString();
                if (!string.IsNullOrEmpty(assetSystemGeneratedId))
                {
                    model.AssetId = assetSystemGeneratedId;
                }
            }
            return model;


        }
        public async Task<ActionResult> AssetWorkHistory(string id)
        {
            var filter = new WorkOrderSearchViewModel()
            {
                AssetId = id
            };
            var vm = await SetCrudListViewModel(filter);
            return DataTableIndexView(vm);
        }

        [HttpGet]
        public async Task<ActionResult> Dashboard()
        {
            var workOrderRecords = await _dashboardService.GetDashboardData();
            if (Request.IsAjaxRequest())
            {
                return PartialView("~/Views/WorkOrder/Dashboard/Dashboard.cshtml", workOrderRecords);
            }
            return View("~/Views/WorkOrder/Dashboard/Dashboard.cshtml", workOrderRecords);
        }

        [HttpGet]
        public async Task<ActionResult> GanttChart()
        {
            var workOrderRecords = await _dashboardService.GetDashboardData();

            var model = new WorkOrderGanttChartViewModel();
            model.WorkOrders = workOrderRecords;
            return View("~/Views/WorkOrder/_GanttChart.cshtml", model);
        }

        public override List<DataTableViewModel> GetColumns()
        {
            return new List<DataTableViewModel>()
            {
               // new DataTableViewModel{title = "<input type='checkbox' class='select-all-checkbox' onclick='selectAllCheckBoxChanged(this)'>",className="action text-right exclude-from-export", data = "Id"},
                new DataTableViewModel{title = "Status",data = "Status",format="html",formatValue="status",orderable=true },
                new DataTableViewModel{title = "ID #",data = "SystemGeneratedId", orderable=true},
                new DataTableViewModel{title = "Asset Id",data = "Asset.FormattedSystemGeneratedId",orderable=true,filterId="asset-id-search-container",hasFilter=true, sortingColumn = "Asset.SystemGeneratedId"},
                new DataTableViewModel{title = "Asset Type",data = "AssetTypeName",orderable=true,filterId="asset-type-search-container",hasFilter=true},
                new DataTableViewModel{title = "Description",format="html",formatValue="tooltip",data = "Description",orderable=true},
                new DataTableViewModel{title = "Street",data = "Intersection", orderable = true},
                new DataTableViewModel{title = "Work Step",data = "Task",orderable=true,filterId="task-search-conatiner",hasFilter=true},
                new DataTableViewModel{title = "Urgency",data = "Urgency",orderable=true,filterId="urgency-search-container",hasFilter=true},
                new DataTableViewModel{title = "Manager",data = "Manager.Name",orderable=true,filterId="manager-search-container",hasFilter= true},
                new DataTableViewModel{title = "Hours",data = "ActualHours",orderable=true},
                new DataTableViewModel{title = "Labor",data = "LabourCost",orderable = true, className="dt-currency"},
                new DataTableViewModel{title = "Material",data = "MaterialCost",orderable=true,className="dt-currency"},
                new DataTableViewModel{title = "Equipment",data = "EquipmentCost",orderable=true,className="dt-currency"},
                new DataTableViewModel{title = "Due Date",data = "FormattedDueDate", orderable = true, sortingColumn = "DueDate"},
                new DataTableViewModel{title = "Photos",data = "ImageUrls", format = "html", formatValue="multiple-images", className = "image-thumbnail action"},
                new DataTableViewModel{title = "Action",data = null,className="action text-right exclude-form-export"}

            };
        }

        protected override async void SetDatatableActions<T>(DatatablePaginatedResultModel<T> result)
        {
            result.ActionsList = new List<DataTableActionViewModel>()
            {
                //new DataTableActionViewModel() {Action="TasksIndex",Title="Add",Href=$"/WorkOrder/TasksIndex/{{Id}}" },
                new DataTableActionViewModel() {Tooltip="Approval", Action="ApprovalIndex",Title="Timesheet",Href=$"/WorkOrders/Index/?activeTab=timesheet-approval&id={{Id}}", DatatableHrefType = DatatableHrefType.Link },
                //new DataTableActionViewModel() {Tooltip="Notes", Action="GetNotes",Title="Notes",Class="@HasNotesClass",Href=$"/WorkOrder/GetNotes/{{Id}}" },
                //new DataTableActionViewModel() {Tooltip="Update",Action="Update",Title="Update",Href=$"/WorkOrder/Update/{{Id}}"},
                new DataTableActionViewModel() {Tooltip="Update Status",Action="UpdateStatus",Title="UpdateStatus",Href=$"/WorkOrder/UpdateStatus/{{Id}}",DisableBasedOn=new DisableModel {Property="CantUpdateStatus",Value="true" } },
                new DataTableActionViewModel() {Tooltip="Print Work Order", Action = "Print", Title = "Print", Href = $"#", Class = "print-work-order", Attr = new List<string> { "Id" }, DatatableHrefType=DatatableHrefType.Link}

            };
        }

        protected override async Task<CrudListViewModel> OverrideCrudListVM(CrudListViewModel vm)
        {
            var html = "";
            html += @"
                    <div class=""col d-flex justify-content-end mb-2"">
                        <div class=""p-2 d-flex"">
                            <span class=""custom-badge Approved m-1""> </span>
                            <span class=""stat-name"">Approved </span>
                        </div>
                        <div class=""p-2 d-flex"">
                            <span class=""custom-badge OnHold m-1""> </span>
                            <span class=""stat-name"">On Hold </span>
                        </div>
                        <div class=""p-2 d-flex"">
                            <span class=""custom-badge Complete m-1""> </span>
                            <span class=""stat-name"">Complete </span>
                        </div>
                        <div class=""m-2 d-flex"">
                            <span class=""custom-badge Working m-1""> </span>
                            <span class=""stat-name"">Working</span>
                        </div>
                       <div class=""m-2 d-flex"">
                            <span class=""custom-badge Open m-1""> </span>
                            <span class=""stat-name"">Open</span>
                        </div>
                    </div>
                   ";
            html = "";
            vm.SearchBarHtml = html;
            vm.TitleHtml = @"
            <div class=""d-flex justify-content-start"">
                <h3 class=""page-title text-site-primary m-md-0"">Work Order</h3>
                <a class=""btn btn-sm btn-site-primary ms-4 me-2"" target=""_blank"" href=""/StreetServiceRequest"">
                   Works Service Request
                </a>
                <a class=""btn btn-sm btn-site-primary ms-1 me-2"" target=""_blank"" href=""/Approval"">
                   Timesheet Approvals
                </a>
            </div>";
            vm.HideTopSearchBar = true;
            vm.HideTitle = true;
            vm.IsLayoutNull = true;
            vm.LoadDatatableScript = false;
            vm.RowClickDefaultAction = "Update";
            return vm;
        }

        public override ActionResult DataTableIndexView(CrudListViewModel vm)
        {
            return View("_Index", vm);
        }

        public async override Task<ActionResult> Create()
        {
            var model = new WorkOrderModifyViewModel();
            var dynamicColumns = await _dynamicColumnService.GetDynamicColumns(DynamicColumnEntityType.WorkOrder, -1);
            model.DynamicColumns = dynamicColumns;
            var updateVm = GetUpdateViewModel("Create", model);
            updateVm = await OverrideUpdateViewModel(updateVm);
            return UpdateView(updateVm);
        }

        public async Task<ActionResult> CreateWorkOrder(int id = 0)
        {
            var model = await PrepareWorkOrderModel(id, false);
            var updateVm = GetUpdateViewModel("Create", model);
            updateVm = await OverrideUpdateViewModel(updateVm);
            return UpdateView(updateVm);
        }

        public override async Task<ActionResult> Update(int id)
        {
            try
            {
                var response = await _service.GetById(id);
                WorkOrderModifyViewModel model = null;
                AssetDetailViewModel assetDetailModel = new();
                WorkOrderDetailViewModel detailModel = null;
                if (response.Status == HttpStatusCode.OK)
                {
                    var parsedModel = response as RepositoryResponseWithModel<WorkOrderDetailViewModel>;
                    detailModel = parsedModel.ReturnModel;
                    model = _mapper.Map<WorkOrderModifyViewModel>(parsedModel.ReturnModel);
                }
                if (model != null)
                {
                    if (detailModel?.Asset?.Id > 0)
                    {
                        assetDetailModel = await GetAssetById(detailModel.Asset.Id ?? 0);
                    }
                    var crudUpdateModel = GetUpdateViewModel("Update", model);
                    crudUpdateModel.UpdateViewPath = "_UpdateWorkOrderTab";
                    ViewBag.WorkOrderId = model.Id;
                    var notes = await GetWorkOrderNotes(id);
                    //var assetApprovalWorkHistory = await GetAssetApprovedWorkOrderData(model.Id);
                    var updateVM = new WorkOrderTabsModifyViewModel()
                    {
                        CrudUpdateViewModel = crudUpdateModel,
                        Notes = notes,
                        WorkOrderDetailModel = detailModel,
                        AssetDetailModel = assetDetailModel
                    };
                    return View("_Update", updateVM);
                }
                else
                {

                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
        }

        public async Task<ActionResult> CreateWorkOrderForStreetService(int id = 0)
        {
            var model = await PrepareWorkOrderModel(id, true);
            var updateVm = GetUpdateViewModel("Create", model);
            updateVm = await OverrideUpdateViewModel(updateVm);
            return UpdateView(updateVm);
        }

        private async Task<WorkOrderModifyViewModel> PrepareWorkOrderModel(int id, bool isStreetService)
        {
            var model = new WorkOrderModifyViewModel();

            if (id != 0)
            {
                var dynamicColumns = await _dynamicColumnService.GetDynamicColumns(DynamicColumnEntityType.WorkOrder, 0);
                model.DynamicColumns = dynamicColumns;

                if (isStreetService)
                {
                    model.StreetServiceRequestId = id;
                    ViewBag.AssetId = id;
                }
                else
                {
                    var asset = await GetAssetById(id);
                    if (asset != null)
                    {
                        model.Asset = new ViewModels.WorkOrder.WOAssetViewModel
                        {
                            Id = id,
                            SystemGeneratedId = asset.SystemGeneratedId,
                            Description = asset.Description,
                            AssetType = asset.AssetType.Name,
                            AssetTypeId = asset.AssetType.Id,
                            Street = asset.Intersection,
                        };
                        ViewBag.AssetId = id;
                    }
                }
            }

            return model;
        }

        public override Task<ActionResult> Create(WorkOrderModifyViewModel model)
        {
            model.Manager.Id = long.Parse(_userInfoService.LoggedInUserId());
            return base.Create(model);
        }



        public async Task<ActionResult> GetNotes(int id)
        {
            try
            {
                var notes = await GetWorkOrderNotes(id);
                ViewBag.WorkOrderId = id;
                return View("_Notes", notes);
            }
            catch (Exception ex) { _logger.LogError($"WorkOrder Notes method threw an exception, Message: {ex.Message}"); return RedirectToAction("Index"); }
        }
        public async Task<ActionResult> GetNotesListPartialView(long id)
        {
            try
            {
                var notes = await GetWorkOrderNotes(id);
                return View("~/Views/Shared/Notes/_NotesListPartialView.cshtml", notes);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Asset Notes method threw an exception, Message: {ex.Message}");
                return RedirectToAction("Index");
            }
        }
        public async Task<ActionResult> SaveNotes(WorkOrderNotesViewModel model)
        {
            var response = await _service.SaveNotes(model);
            return Json(response.Status == HttpStatusCode.OK);
        }

        public async Task<ActionResult> TasksIndex(int id)
        {
            var workOrderTasksList = await _service.GetWorkOrderTasks(id);
            ViewBag.WorkOrderId = id;
            return View("_TasksIndex", workOrderTasksList);
        }

        public async Task<ActionResult> AddTasks(int id)
        {
            try
            {
                var model = new WorkOrderTasksModifyViewModel();
                model.WorkOrderId = id;
                return View("_AddTasks", model);
            }
            catch (Exception ex) { _logger.LogError($"WorkOrder AddTasks method threw an exception, Message: {ex.Message}"); return RedirectToAction("Index"); }
        }

        public virtual async Task<ActionResult> UpdateStatus(int id)
        {
            var workOrder = new WorkOrderModifyStatusViewModel();
            try
            {
                var response = await _service.GetById(id);

                if (response.Status == HttpStatusCode.OK)
                {
                    var workOrderResponse = response as RepositoryResponseWithModel<WorkOrderDetailViewModel>;
                    workOrder = _mapper.Map<WorkOrderModifyStatusViewModel>(workOrderResponse.ReturnModel);
                }
            }
            catch (Exception ex)
            {
            }
            return PartialView("_UpdateStatus", workOrder);
        }

        [HttpPost]
        public virtual async Task<ActionResult> UpdateStatus(WorkOrderModifyStatusViewModel model)
        {
            try
            {
                var response = await _service.UpdateStatus(model);

                if (response.Status == HttpStatusCode.OK)
                {
                    return Json(new { Status = true });
                }
            }
            catch (Exception ex)
            {
            }
            return Json(new { Status = false });
        }

        [HttpPost]
        public async Task<ActionResult> CreateTasks(WorkOrderTasksModifyViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var response = await _service.CreateTasks(model);
                    return Json(new { status = response });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"CheckList Create method threw an exception, Message: {ex.Message}");
            }
            var invalidFieldErrors = ModelState.Where(x => x.Value.Errors.Any())
              .ToDictionary(
                  kvp => kvp.Key,
                  kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
              );
            return Json(new { status = false, fieldErrors = invalidFieldErrors });
        }

        public async Task<long> _SetWorkOrderTaskStatus(long Id, WOTaskStatusCatalog Status)
        {
            try
            {
                var checkListId = await _service.SetWorkOrderTaskStatus(Id, Status);
                return checkListId;
            }
            catch (Exception ex)
            {
                _logger.LogError($"WorkOrder _SetWorkOrderTaskStatus method threw an exception, Message: {ex.Message}");
                return -1;
            }

        }

        public async Task<ActionResult> _GetAttachmentView(long Id)
        {
            try
            {
                var attachments = await _service.GetWorkOrderAttachments(Id);
                return View("~/Views/Shared/GalleryPartialView/_GetGalleryView.cshtml", attachments);
            }
            catch (Exception ex)
            {
                _logger.LogError($"WorkOrder _GetGalleryView method threw an exception, Message: {ex.Message}");
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
                _logger.LogError($"WorkOrder _GetAttachmentUrl method threw an exception, Message: {ex.Message}");
                return null;
            }
        }

        public IActionResult _LabourSectionRow(WorkOrderLaborModifyViewModel model, int rowNumber)
        {
            ViewData["RowNumber"] = rowNumber;
            return PartialView("_LabourSectionRow", model);
        }

        public IActionResult _MaterialSectionRow(WorkOrderMaterialModifyViewModel model, int rowNumber)
        {
            ViewData["RowNumber"] = rowNumber;
            return PartialView("_MaterialSectionRow", model);
        }

        public IActionResult _EquipmentSectionRow(WorkOrderEquipmentModifyViewModel model, int rowNumber)
        {
            ViewData["RowNumber"] = rowNumber;
            return PartialView("_EquipmentSectionRow", model);
        }

        public IActionResult _TechnicianSectionRow(WorkOrderTechnicianModifyViewModel model, int rowNumber)
        {
            ViewData["RowNumber"] = rowNumber;
            return PartialView("_TechnicianSectionRow", model);
        }


        public async Task<ActionResult> Print(long id)
        {
            var responseData = await _service.GetById(id);
            var data = responseData as RepositoryResponseWithModel<WorkOrderDetailViewModel>;
            return View("Print", data.ReturnModel);
        }

        protected override WorkOrderSearchViewModel SetSelect2CustomParams(string customParams)
        {
            var model = base.SetSelect2CustomParams(customParams);
            model.NotStatus = WOStatusCatalog.Approved;
            return model;
        }

        private async Task<List<INotesViewModel>> GetWorkOrderNotes(long id)
        {
            var response = await _service.GetNotesByWorkOrderId(id);
            var model = new List<WorkOrderNotesViewModel>();
            if (response.Status == HttpStatusCode.OK)
            {
                var responseModel = response as RepositoryResponseWithModel<List<WorkOrderNotesViewModel>>;
                model = responseModel.ReturnModel;
            }
            List<INotesViewModel> notesViewModels = model.Cast<INotesViewModel>().ToList();
            return notesViewModels;
        }
        private async Task<AssetDetailViewModel> GetAssetById(long id)
        {
            try
            {
                var asset = await _assetService.GetById(id);
                if (asset.Status == HttpStatusCode.OK)
                {
                    var parsedModel = asset as RepositoryResponseWithModel<AssetDetailViewModel>;
                    return parsedModel.ReturnModel;
                }
            }
            catch (Exception ex)
            {

            }
            return null;
        }

    }
}

