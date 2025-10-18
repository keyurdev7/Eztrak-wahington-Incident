using Centangle.Common.ResponseHelpers.Models;
using Pagination;
using ViewModels;
using ViewModels.Charts;
using ViewModels.Dashboard.Common.Card;
using ViewModels.Report;
using ViewModels.Report.PendingOrder;
using ViewModels.Report.RawReport;

namespace Repositories.Services.Report.Interface
{
    public interface IReportService
    {
        Task<ChartResponseViewModel> GetWorkOrderByManagerChartData(WorkOrderByManagerChartSearchViewModel search);
        Task<ChartResponseViewModel> GetWorkOrderByTechnicianChartData(WorkOrderByManagerChartSearchViewModel search);
        Task<ChartResponseViewModel> GetWorkOrderChartData(WorkOrderChartSearchViewModel search);
        Task<ChartResponseViewModel> GetWorkOrderByAssetTypeChartData(WorkOrderChartSearchViewModel search);
        Task<ChartResponseViewModel> GetWorkOrderByRepairTypeChartData(WorkOrderChartSearchViewModel search);
        Task<IRepositoryResponse> GetWorkOrderReportData(IBaseSearchModel search);
        Task<IRepositoryResponse> GetPendingOrderReportData(PendingOrderReportSearchViewModel search);
        Task<CardViewModel> GetAverageOrderCompletionTime(PendingOrderReportSearchViewModel search);

        Task<ChartResponseViewModel> GetAssetsByConditionChartData(WorkOrderByManagerChartSearchViewModel search);
        Task<ChartResponseViewModel> GetAssetsMaintenanceDueChartData(WorkOrderByManagerChartSearchViewModel search);
        Task<ChartResponseViewModel> GetAssetsReplacementDueChartData(WorkOrderByManagerChartSearchViewModel search);

        Task<CardViewModel> GetAverageLaborCost(WorkOrderByManagerChartSearchViewModel search);
        Task<CardViewModel> GetAverageEquipmentCost(WorkOrderByManagerChartSearchViewModel search);
        Task<CardViewModel> GetAverageMaterialCost(WorkOrderByManagerChartSearchViewModel search);
        Task<ChartResponseViewModel> GetCostAccuracy(WorkOrderByManagerChartSearchViewModel search);

        Task<IRepositoryResponse> GetTransactionReport(TransactionSearchViewModel model);
        Task<IRepositoryResponse> GetTimesheetReport(IBaseSearchModel search);

        Task<IRepositoryResponse> GetWorkOrderRawReportData(IBaseSearchModel filters);
        Task<WorkOrderRawReportColumns> GetWorkOrderRawReportColumnCount();

        Task<List<string>> GetAssetRawReportColumns();
        Task<IRepositoryResponse> GetAssetsRawReport(AssetSearchViewModel search);
    }
}
