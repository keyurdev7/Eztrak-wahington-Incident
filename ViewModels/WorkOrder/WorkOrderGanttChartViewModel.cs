using ViewModels.Report.WorkOrder;

namespace ViewModels
{
    public class WorkOrderGanttChartViewModel
    {
        public List<WorkOrderDashboardViewModel> WorkOrders { get; set; } = new();
    }
}
