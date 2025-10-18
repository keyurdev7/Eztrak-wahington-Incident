using System.Diagnostics;

namespace ViewModels.Timesheet
{
    public class TimesheetHoursBreakdownViewModel
    {
        public TimesheetHoursBreakdownViewModel()
        {

        }
        public TimesheetHoursBreakdownViewModel(DayOfWeek day, double hours)
        {
            Day = day;
            TotalHours = hours;
            timeSheetHoursLimit = GetHoursLimits();
        }

        private List<TimesheetHoursLimit> timeSheetHoursLimit = new List<TimesheetHoursLimit>();

        public DayOfWeek Day { get; set; }
        public double TotalHours { get; set; }
        public double STRate { get; set; }
        public double OTRate { get; set; }
        public double DTRate { get; set; }
        public double STHours { get => DisectByRange(GetHourRangeByDay()?.STHourLimit); }
        public double OTHours { get => DisectByRange(GetHourRangeByDay()?.OTTHourLimit); }
        public double DTHours { get => DisectByRange(GetHourRangeByDay()?.DTTHourLimit); }

        public double TotalCost
        {
            get
            {
                return ((STHours * STRate) + (OTHours * OTRate) + (DTHours * DTRate));
            }
        }
        private TimesheetHoursLimit? GetHourRangeByDay()
        {
            return timeSheetHoursLimit.Where(x => x.Day == Day).FirstOrDefault();
        }
        private double DisectByRange(HoursLimit? hoursLimit)
        {
            if (hoursLimit != null)
            {
                var totalHoursInRange = hoursLimit.End - hoursLimit.Start;
                var hoursBeyondRangeLimit = TotalHours - hoursLimit.Start;
                hoursBeyondRangeLimit = hoursBeyondRangeLimit < 0 ? 0 : hoursBeyondRangeLimit;
                if (hoursBeyondRangeLimit > totalHoursInRange)
                    return totalHoursInRange;
                return hoursBeyondRangeLimit;
            }
            return 0;
        }
        private List<TimesheetHoursLimit> GetHoursLimits()
        {
            var weeklyHourLimits = new List<TimesheetHoursLimit>();
            foreach (var day in Enum.GetValues(typeof(DayOfWeek)))
            {
                switch (day)
                {
                    case DayOfWeek.Sunday: weeklyHourLimits.Add(new TimesheetHoursLimit { Day = (DayOfWeek)day, STHourLimit = new HoursLimit(0, 0), OTTHourLimit = new HoursLimit(0, 8), DTTHourLimit = new HoursLimit(8, 24) }); break;
                    case DayOfWeek.Saturday: weeklyHourLimits.Add(new TimesheetHoursLimit { Day = (DayOfWeek)day, STHourLimit = new HoursLimit(0, 0), OTTHourLimit = new HoursLimit(0, 24), DTTHourLimit = new HoursLimit(0, 0) }); break;
                    case DayOfWeek.Friday: weeklyHourLimits.Add(new TimesheetHoursLimit { Day = (DayOfWeek)day, STHourLimit = new HoursLimit(0, 0), OTTHourLimit = new HoursLimit(0, 24), DTTHourLimit = new HoursLimit(0, 0) }); break;
                    default: weeklyHourLimits.Add(new TimesheetHoursLimit { Day = (DayOfWeek)day, STHourLimit = new HoursLimit(0, 10), OTTHourLimit = new HoursLimit(10, 12), DTTHourLimit = new HoursLimit(12, 24) }); break;
                }
            }
            return weeklyHourLimits;
        }
    }

    public class TimesheetHoursLimit
    {
        public DayOfWeek Day { get; set; }
        public HoursLimit STHourLimit { get; set; } = new HoursLimit(0, 10);
        public HoursLimit OTTHourLimit { get; set; } = new HoursLimit(10, 12);
        public HoursLimit DTTHourLimit { get; set; } = new HoursLimit(12, 24);
    }
    public class HoursLimit
    {
        public HoursLimit(double start, double end)
        {
            Start = start;
            End = end;
        }

        public double Start { get; set; }
        public double End { get; set; }
    }
}
