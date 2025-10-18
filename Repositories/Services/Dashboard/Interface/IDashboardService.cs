using ViewModels.Charts;
using ViewModels.Dashboard;
using ViewModels.Report.PendingOrder;
using ViewModels.Report.WorkOrder;

namespace Repositories.Services.Dashboard.Interface
{
    public interface IDashboardService
    {
        Task<ChartResponseViewModel> GetPendingOrderChartData(PendingOrderChartSearchViewModel search);
        Task<bool> ValidatePassword(string password);
        Task<List<WorkOrderDashboardViewModel>> GetDashboardData();
    }
}
