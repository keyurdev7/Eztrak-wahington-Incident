using ViewModels.Timesheet;

namespace Repositories
{
    public interface ITimesheetLimit
    {
        TimesheetHoursBreakdownViewModel GetHoursBreakdown(DayOfWeek day, double hours);

    }
}
