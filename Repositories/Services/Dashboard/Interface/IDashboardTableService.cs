using ViewModels.Dashboard.Common.Table;
using ViewModels.Report.PendingOrder;

namespace Repositories.Services.Dashboard.Interface
{
    public interface IDashboardTableService
    {
        Task<DashboardTableDataViewModel<PendingOrderReportViewModel>> GetPendingOrderTableData();

        //Datatable cell save
        Task<bool> SaveDatatableCell(string propertyName, string propertyValue, string entityId, string entityName);
    }
}
