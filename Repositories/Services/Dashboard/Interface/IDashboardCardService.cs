using ViewModels.Charts;
using ViewModels.Dashboard.Common.Card;

namespace Repositories.Services.Dashboard.Interface
{
    public interface IDashboardCardService
    {
        Task<DashboardCardDataViewModel> GetPendingOrderCardData();
    }
}
