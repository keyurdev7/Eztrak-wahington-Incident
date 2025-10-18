using Enums;
using Models.Models.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class TimesheetBreakdown : BaseDBModel
    {
        public DateTime Date { get; set; }
        public DayOfWeek Day { get; set; }

        public bool IncludePerDiem { get; set; }
        public double RegularHours { get; set; }
        public double OvertimeHours { get; set; }
        public double DoubleTimeHours { get; set; }
        public TSRefStatus? TSRefStatus { get; set; }
        public double TSRefNumber { get; set; }
        public PaymentIndicatorStatus? PaymentIndicator { get; set; }
        public double DailyPerDiem { get; set; }

        public bool IsOnSite { get; set; } = true;

        [ForeignKey("Timesheet")]
        public long TimesheetId { get; set; }
        public Timesheet Timesheet { get; set; }
    }
}
