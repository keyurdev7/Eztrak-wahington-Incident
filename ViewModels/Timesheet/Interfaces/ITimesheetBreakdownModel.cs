using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels.Timesheet.Interfaces
{
    public interface ITimesheetBreakdownModel
    {
        public DayOfWeek Day { get; set; }
        public bool DisableDayEntry { get; set; }
        //public bool DisableDayPerDiemEntry { get; set; }
    }
}
