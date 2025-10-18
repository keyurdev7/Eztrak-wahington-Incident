using ViewModels.CRUD;
using ViewModels.DataTable;
using ViewModels.Report.Common;
using ViewModels.Report.RawReport;

namespace ViewModels.Report.Factory.interfaces
{
    public interface IReportFactory
    {
        ReportViewModel CreatePendingOrderReportViewModel();
        List<DataTableViewModel> GetPendingOrderReportColumns();
        ReportCrudListViewModel CreateWorkOrderReportViewModel();
        ReportCrudListViewModel CreateMaintenanceReportViewModel();
        ReportCrudListViewModel CreateMaterialCostReportViewModel();
        ReportCrudListViewModel CreateReplacementReportViewModel();
        ReportCrudListViewModel CreateEquipmentCostReportViewModel();
        ReportCrudListViewModel CreateTransactionReportViewModel();
        ReportCrudListViewModel CreateTimesheetReportViewModel();
        ReportCrudListViewModel CreateWorkOrderRawReportViewModel(WorkOrderRawReportColumns additionalColumns);
        ReportCrudListViewModel CreateAssetRawReportViewModel(List<string> additionalColumns);
    }
}