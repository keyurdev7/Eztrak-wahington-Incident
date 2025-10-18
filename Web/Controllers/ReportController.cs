using System.Net;
using System.Runtime.CompilerServices;
using AutoMapper;
using Centangle.Common.ResponseHelpers.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Newtonsoft.Json;
using Pagination;
using Repositories.Common;
using Repositories.Services.Report.Interface;
using ViewModels;
using ViewModels.CRUD;
using ViewModels.DataTable;
using ViewModels.Report.Factory.interfaces;
using ViewModels.Report.RawReport;
using ViewModels.Shared;
using ViewModels.Timesheet;

namespace Web.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {
        private readonly ILogger<ReportController> _logger;
        private readonly IReportService _reportService;
        private readonly IReportFactory _reportFactory;
        private readonly IAssetService<AssetModifyViewModel, AssetModifyViewModel, AssetDetailViewModel> _assetService;
        private readonly IInventoryService<InventoryModifyViewModel, InventoryModifyViewModel, InventoryDetailViewModel> _inventoryService;
        private readonly IEquipmentService<EquipmentModifyViewModel, EquipmentModifyViewModel, EquipmentDetailViewModel> _equipmentService;
        private readonly IMapper _mapper;
        private readonly IUserSearchSettingService<UserSearchSettingModifyViewModel, UserSearchSettingModifyViewModel, UserSearchSettingDetailViewModel> _userSearchSettingService;

        public ReportController(
            ILogger<ReportController> logger
            , IReportService reportService
            , IReportFactory reportFactory
            , IAssetService<AssetModifyViewModel, AssetModifyViewModel, AssetDetailViewModel> assetService
            , IInventoryService<InventoryModifyViewModel, InventoryModifyViewModel, InventoryDetailViewModel> inventoryService
            , IEquipmentService<EquipmentModifyViewModel, EquipmentModifyViewModel, EquipmentDetailViewModel> equipmentService
            , IMapper mapper
            , IUserSearchSettingService<UserSearchSettingModifyViewModel, UserSearchSettingModifyViewModel, UserSearchSettingDetailViewModel> userSearchSettingService
            )
        {
            _logger = logger;
            _reportService = reportService;
            _reportFactory = reportFactory;
            _assetService = assetService;
            _inventoryService = inventoryService;
            _equipmentService = equipmentService;
            _mapper = mapper;
            _userSearchSettingService = userSearchSettingService;
        }


        public async Task<ActionResult> WorkOrder()
        {
            var vm = _reportFactory.CreateWorkOrderReportViewModel();
            return View("~/Views/Report/_Index.cshtml", vm);
        }

        public async Task<JsonResult> WorkOrderSearch(WorkOrderSearchViewModel searchModel)
        {
            var response = await _reportService.GetWorkOrderReportData(searchModel);
            PaginatedResultModel<WorkOrderDetailViewModel> model = new();
            if (response.Status == System.Net.HttpStatusCode.OK)
            {
                var parsedResponse = response as RepositoryResponseWithModel<PaginatedResultModel<WorkOrderDetailViewModel>>;
                model = parsedResponse?.ReturnModel ?? new();
            }
            return await ProcessSearchResult(searchModel, model, _reportFactory.CreateWorkOrderReportViewModel().ActionsList);
        }

        public async Task<ActionResult> Maintenance()
        {
            var vm = _reportFactory.CreateMaintenanceReportViewModel();
            return View("~/Views/Report/_Index.cshtml", vm);
        }

        public async Task<JsonResult> MaintenanceSearch(AssetSearchViewModel searchModel)
        {

            searchModel.DisablePagination = true;
            var response = await _assetService.GetAll<AssetDetailViewModel>(searchModel);
            PaginatedResultModel<AssetDetailViewModel> model = new();
            if (response.Status == System.Net.HttpStatusCode.OK)
            {
                var parsedResponse = response as RepositoryResponseWithModel<PaginatedResultModel<AssetDetailViewModel>>;
                model = parsedResponse?.ReturnModel ?? new();
            }
            var reportModel = _mapper.Map<PaginatedResultModel<MaintenanceReportViewModel>>(model);
            reportModel.Items = reportModel.Items.Where(x => x.MonthsRemainingInNextMaintenance < 7).ToList();
            return await ProcessSearchResult(searchModel, reportModel, _reportFactory.CreateMaintenanceReportViewModel().ActionsList);
        }

        public async Task<ActionResult> MaterialCost()
        {
            var vm = _reportFactory.CreateMaterialCostReportViewModel();
            return View("~/Views/Report/_Index.cshtml", vm);
        }

        public async Task<JsonResult> MaterialCostSearch(InventorySearchViewModel searchModel)
        {

            var response = await _inventoryService.GetAll<InventoryDetailViewModel>(searchModel);
            PaginatedResultModel<InventoryDetailViewModel> model = new();
            if (response.Status == System.Net.HttpStatusCode.OK)
            {
                var parsedResponse = response as RepositoryResponseWithModel<PaginatedResultModel<InventoryDetailViewModel>>;
                model = parsedResponse?.ReturnModel ?? new();
            }
            return await ProcessSearchResult(searchModel, model, _reportFactory.CreateMaterialCostReportViewModel().ActionsList);
        }

        public async Task<ActionResult> Replacement()
        {
            var vm = _reportFactory.CreateReplacementReportViewModel();
            return View("~/Views/Report/_Index.cshtml", vm);
        }

        public async Task<JsonResult> ReplacementSearch(AssetSearchViewModel searchModel)
        {

            searchModel.DisablePagination = true;
            var response = await _assetService.GetAll<AssetDetailViewModel>(searchModel);
            PaginatedResultModel<AssetDetailViewModel> model = new();
            if (response.Status == System.Net.HttpStatusCode.OK)
            {
                var parsedResponse = response as RepositoryResponseWithModel<PaginatedResultModel<AssetDetailViewModel>>;
                model = parsedResponse?.ReturnModel ?? new();
            }
            var reportModel = _mapper.Map<PaginatedResultModel<RepairingReportViewModel>>(model);
            reportModel.Items = reportModel.Items.Where(x => x.MonthsRemainingInNextRepair < 7).ToList();
            return await ProcessSearchResult(searchModel, reportModel, _reportFactory.CreateReplacementReportViewModel().ActionsList);
        }

        public async Task<ActionResult> EquipmentCost()
        {
            var vm = _reportFactory.CreateEquipmentCostReportViewModel();
            return View("~/Views/Report/_Index.cshtml", vm);
        }

        public async Task<JsonResult> EquipmentCostSearch(EquipmentSearchViewModel searchModel)
        {

            var response = await _equipmentService.GetAll<EquipmentDetailViewModel>(searchModel);
            PaginatedResultModel<EquipmentDetailViewModel> model = new();
            if (response.Status == System.Net.HttpStatusCode.OK)
            {
                var parsedResponse = response as RepositoryResponseWithModel<PaginatedResultModel<EquipmentDetailViewModel>>;
                model = parsedResponse?.ReturnModel ?? new();
            }
            return await ProcessSearchResult(searchModel, model, _reportFactory.CreateEquipmentCostReportViewModel().ActionsList);
        }

        public async Task<ActionResult> Transaction()
        {
            var vm = _reportFactory.CreateTransactionReportViewModel();
            return View("~/Views/Report/_Index.cshtml", vm);
        }

        public async Task<JsonResult> TransactionSearch(TransactionSearchViewModel searchModel)
        {

            var response = await _reportService.GetTransactionReport(searchModel);
            PaginatedResultModel<TransactionReportViewModel> model = new();
            if (response.Status == System.Net.HttpStatusCode.OK)
            {
                var parsedResponse = response as RepositoryResponseWithModel<PaginatedResultModel<TransactionReportViewModel>>;
                model = parsedResponse?.ReturnModel ?? new();
            }
            return await ProcessSearchResult(searchModel, model, _reportFactory.CreateTransactionReportViewModel().ActionsList);
        }

        public async Task<ActionResult> Timesheet()
        {
            var vm = _reportFactory.CreateTimesheetReportViewModel();
            return View("~/Views/Report/_Index.cshtml", vm);
        }

        public async Task<JsonResult> TimesheetSearch(TimeSheetBreakdownReportSearchViewModel searchModel)
        {
            var response = await _reportService.GetTimesheetReport(searchModel);
            PaginatedResultModel<TimesheetBreakdownDetailForReportViewModel> model = new();
            if (response.Status == HttpStatusCode.OK)
            {
                var parsedResponse = response as RepositoryResponseWithModel<PaginatedResultModel<TimesheetBreakdownDetailForReportViewModel>>;
                model = parsedResponse?.ReturnModel ?? new();
            }
            return await ProcessSearchResult(searchModel, model, _reportFactory.CreateTimesheetReportViewModel().ActionsList);
        }

        public async Task<ActionResult> WorkOrderRaw()
        {
            var additionalColumns = await _reportService.GetWorkOrderRawReportColumnCount();
            var vm = _reportFactory.CreateWorkOrderRawReportViewModel(additionalColumns);
            return View("~/Views/Report/_Index.cshtml", vm);
        }

        public async Task<JsonResult> WorkOrderRawSearch(WorkOrderSearchViewModel searchModel)
        {
            var response = await _reportService.GetWorkOrderRawReportData(searchModel);
            PaginatedResultModel<WorkOrderRawReportViewModel> model = new();
            if (response.Status == HttpStatusCode.OK)
            {
                var parsedResponse = response as RepositoryResponseWithModel<PaginatedResultModel<WorkOrderRawReportViewModel>>;
                model = parsedResponse?.ReturnModel ?? new();
            }
            return await ProcessSearchResult(searchModel, model, new List<DataTableActionViewModel>());
        }

        public async Task<ActionResult> AssetRaw()
        {
            var additionalColumns = await _reportService.GetAssetRawReportColumns();
            var vm = _reportFactory.CreateAssetRawReportViewModel(additionalColumns);
            return View("~/Views/Report/_Index.cshtml", vm);
        }

        public async Task<JsonResult> AssetRawSearch(AssetSearchViewModel searchModel)
        {
            var response = await _reportService.GetAssetsRawReport(searchModel);
            PaginatedResultModel<AssetRawReportViewModel> model = new();
            if (response.Status == HttpStatusCode.OK)
            {
                var parsedResponse = response as RepositoryResponseWithModel<PaginatedResultModel<AssetRawReportViewModel>>;
                model = parsedResponse?.ReturnModel ?? new();
            }
            return await ProcessSearchResult(searchModel, model, new List<DataTableActionViewModel>());
        }

        //Generic Methods

        protected virtual async Task<JsonResult> ProcessSearchResult<T, S>(S searchModel, PaginatedResultModel<T> model, List<DataTableActionViewModel> actionsList)
            where S : class, IBaseSearchModel
            where T : class, new()
        {
            var result = ConvertToDataTableModel(model, searchModel);
            result.ActionsList = actionsList;
            var jsonResult = Json(result);
            return jsonResult;
        }

        protected DatatablePaginatedResultModel<T> ConvertToDataTableModel<T, S>(PaginatedResultModel<T> model, S searchModel)
            where T : class, new()
            where S : IBaseSearchModel
        {
            return new DatatablePaginatedResultModel<T>(searchModel.Draw, model._meta.TotalCount, model.Items);
        }

    }
}