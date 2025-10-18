using ClosedXML.Excel;
using Models;
using Pagination;
using Repositories.Interfaces;
using ViewModels.Timesheet;

namespace Repositories
{
    public interface ITimesheetService : IBaseCrud<TimesheetModifyViewModel, TimesheetModifyViewModel, TimesheetDetailViewModel>
    {
        Task<TimesheetProjectsViewModel> GetWorkOrdersByTechnicianId(long id);
        Task<long> ModifyApiTimeSheetBreakdowns(TimesheetBreakdownUpdateViewModel model);
        Task<TimeSheetWithBreakdownViewModel> GetTimeSheetBreakdowns(long id);
        Task<TimesheetBriefViewModel> GetTimesheet(TimesheetEmployeeSearchViewModel model);
        Task ApproveTimesheets(List<long> ids, bool Status);
        Task<List<long>> GetApprovedTimesheetIds();
        Task UpdateTimeSheetBreakdowns(TimeSheetWithBreakdownViewModel model);
        Task<List<TimesheetBriefViewModel>> GetAllExistingTimesheetForWeek(long employeeId, DateTime date);
        Task<XLWorkbook> DownloadExcel(List<TimeSheetExcelVM> list);
        Task<long> ModifyTimesheet(TimesheetWebUpdateViewModel model);
        Task<PaginatedResultModel<T>> GetPayPeriodDates<T>(PayPeriodSearchViewModel svm);
        Task<TimesheetBreakdownResForModalVM> UpdateModalHeaderValues(List<TimesheetBreakdownForModalVM> model);
        Task<string> GetWorkOrderName(long id);
        Task<TimesheetBriefViewModel> MapTimesheetVM(Timesheet? timesheet);
    }
}
