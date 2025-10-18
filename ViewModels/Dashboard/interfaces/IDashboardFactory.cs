using ViewModels.Charts.Shared;
using ViewModels.Dashboard.Common.Card;

namespace ViewModels.Dashboard.interfaces
{
    public interface IDashboardFactory
    {
        ChartViewModel CreatePendingOrderChartViewModel();
        ChartViewModel CreateWorkOrderChartViewModel();
        ChartViewModel CreateWorkOrderByAssetTypeChartViewModel();
        ChartViewModel CreateWorkOrderByRepairTypeChartViewModel();
        ChartViewModel CreateWorkOrderByManagerChartViewModel();
        ChartViewModel CreateWorkOrderByTechnicianChartViewModel();
        ChartViewModel CreateAssetByConditionChartViewModel();
        ChartViewModel CreateAssetMaintenanceDueChartViewModel();
        ChartViewModel CreateAssetReplacementDueChartViewModel();
        ChartViewModel CreateCostAccuracyChartViewModel();
    }
}