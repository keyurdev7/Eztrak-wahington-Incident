using ViewModels.Timesheet;

namespace Repositories
{
    public class TimesheetLimitService : ITimesheetLimit
    {
        public TimesheetHoursBreakdownViewModel GetHoursBreakdown(DayOfWeek day, double hours)
        {
            return new TimesheetHoursBreakdownViewModel(day, hours);
        }
    }
}
