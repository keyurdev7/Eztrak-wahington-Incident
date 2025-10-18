using Helpers.Reports.Interfaces;
using Helpers.Reports;
using ViewModels.Report.PendingOrder;

namespace Repositories.Services.Report.Common.interfaces
{
    public interface IReportServiceQueries
    {
        Task<IQueryable<T>> GetPendingOrderReportQuery<T>(PendingOrderReportSearchViewModel search) where T : PendingOrderReportViewModel, IDate, ILabel, new();

        Task<IQueryable<T>> FilterPendingOrderReportQuery<T>(PendingOrderReportSearchViewModel search) where T : PendingOrderReportViewModel, new();
    }
}
