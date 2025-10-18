using DocumentFormat.OpenXml.Office2010.ExcelAc;

using Pagination;
using ViewModels.Charts.Shared;
using ViewModels.Dashboard.Common.Card;
using ViewModels.Dashboard.Common.Table;
using ViewModels.Dashboard.interfaces;

namespace ViewModels.Dashboard
{
    public class DashboardViewModel
    {
        public DashboardViewModel()
        {

        }
       
        public IncidentDashboardViewModel IncidentDashboard { get; set; } = new();
       
        public ChartViewModel PendingOrder { get; set; } = new();
        public ChartViewModel WorkOrder { get; set; } = new();
        public ChartViewModel WorkOrderByAssetType { get; set; } = new();
        public ChartViewModel WorkOrderByRepairType { get; set; } = new();
        public ChartViewModel WorkOrderByTechnician { get; set; } = new();
        public ChartViewModel WorkOrderByManager { get; set; } = new();
        public DashboardCardViewModel AverageCompletionTime { get; set; } = new("average-completion-time", "Average Completion Time", "/Home/GetAverageOrderCompletionTimeCardData");
        public ChartViewModel AssetMaintenanceDue { get; set; } = new();
        public ChartViewModel AssetReplacementDue { get; set; } = new();
        public ChartViewModel AssetByCondition { get; set; } = new();
        public DashboardCardViewModel GetAverageLaborCost { get; set; } = new("average-labor-cost", "Average Labor Cost", "/Home/GetAverageLaborCostCardData");
        public DashboardCardViewModel GetAverageEquipmentCost { get; set; } = new("average-equipment-time", "Average Equipment Cost", "/Home/GetAverageEquipmentCostCardData");
        public DashboardCardViewModel GetAverageMaterialCost { get; set; } = new("average-asset-time", "Average Material Cost", "/Home/GetAverageMaterialCostCardData");
        public ChartViewModel GetCostAccuracy { get; set; } = new();
        public DashboardCardViewModel PendingOrderCardData { get; set; } = new("pending-order-overview", "Pending Order Overview", "/Home/GetPendingOrderCardData");
        public DashboardTableViewModel PendingOrderTableData { get; set; } = new("pending-recent-order", "Recent Pending Order", "/Home/GetPendingOrderTableData");
    }

    public class DashboardSearchViewModel : BaseSearchModel, ITimePeriodSearch
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }

    public class IncidentDashboardViewModel
    {
        public List<string> SeverityLabels { get; set; } = default!;
        public List<long> SeverityCounts { get; set; } = default!;
        public List<string> SeverityColors { get; set; } = default!;
        public List<string> StatusLabels { get; set; } = default!;
        public List<long> StatusCounts { get; set; } = default!;
        public List<string> StatusColors { get; set; } = default!;

        public List<IncidentDashboardStatusReportViewModel> ListIncidentDashboardStatusReport { get; set; } = new();
        public List<IncidentDashboardSeverityReportViewModel> ListIncidentDashboardSeverityReportViewModel { get; set; } = new();

        public List<IncidentDashboardEventTypeReportViewModel> ListIncidentDashboardEventTypeReportViewModel { get; set; } = new();

        public List<IncidentDashboardAssetTypeReportViewModel> ListIncidentDashboardAssetTypeReportViewModel { get; set; } = new();
        public List<IncidentLocationMapViewModel> ListIncidentLocationMapViewModel { get; set; } = new();
        public List<IncidentRecentViewModel> ListIncidentRecentViewModel { get; set; } = new();


        public long TotalIncidentCount { get; set; } = default!;
        public long TotalSeverityCount { get; set; } = default!;
        public long TotalStatusLegendCount { get; set; } = default!;
        public long TotalEventTypeCount { get; set; } = default!;
        public long TotalAssetTypeCount { get; set; } = default!;
        public decimal ResponsePercentage { get; set; } = default!;
        public long TotalSubmittedCount { get; set; } = default!;
        public long TotalValidatedCount { get; set; } = default!;
        public long TotalDispatchedCount { get; set; } = default!;
        public long TotalCompletedCount { get; set; } = default!;
        public long TotalCancelledCount { get; set; } = default!;
    }

    public class IncidentDashboardStatusReportViewModel
    {
        public string name { get; set; } = default!;
        public string color { get; set; } = default!;
        public long count { get; set; } = default!;
        public decimal StatusPercentage { get; set; } = default!;
    }
    public class IncidentDashboardSeverityReportViewModel
    {
        public string name { get; set; } = default!;
        public string color { get; set; } = default!;
        public long count { get; set; } = default!;
        public decimal SeverityPercentage { get; set; } = default!;
    }

    public class IncidentDashboardEventTypeReportViewModel
    {
        public string name { get; set; } = default!;
        public string color { get; set; } = default!;
        public long count { get; set; } = default!;
        public decimal EventTypePercentage { get; set; } = default!;
    }
    public class IncidentDashboardAssetTypeReportViewModel
    {
        public string name { get; set; } = default!;
        public string color { get; set; } = default!;
        public long count { get; set; } = default!;
        public decimal AssetTypePercentage { get; set; } = default!;
    }
    public class IncidentLocationMapViewModel
    {
        public double lat { get; set; }
        public double lon { get; set; }
        public string severity { get; set; } = default!;
        public string incidentStatus { get; set; } = default!;
        public string color { get; set; } = default!;
        public string incidentloc { get; set; } = default!;
        public string calleraddress { get; set; } = default!;
        public string callername { get; set; } = default!;
        public string incidentid { get; set; } = default!;
        public string eventtype { get; set; } = default!;
        public string assettype { get; set; } = default!;
        public string intersection { get; set; } = default!;
        public string description { get; set; } = default!;
        public string perimeter { get; set; } = default!;
        public string callerphone { get; set; } = default!;
    }
    public class IncidentRecentViewModel
    {
        public string incidentId { get; set; } = default!;
        public string incidentloc { get; set; } = default!;
        public string eventtype { get; set; } = default!;
        public string severity { get; set; } = default!;
        public string assettype { get; set; } = default!;
        public string description { get; set; } = default!;
        public string intersection { get; set; } = default!;
        public double lat { get; set; }
        public double lon { get; set; }
        public string incidentstatus { get; set; } = default!;
        public string incidentstatusColor { get; set; } = default!;
        public string perimeter { get; set; } = default!;
    }
}
