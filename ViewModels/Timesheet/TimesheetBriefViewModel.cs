using Enums;
using Models;
using System.Xml.Schema;
using ViewModels.Timesheet.Interfaces;

namespace ViewModels.Timesheet
{
    public class TimesheetEmployeeSearchViewModel
    {
        public long WorkOrderTechnicianId { get; set; }
        public DateTime WeekEnding { get; set; } = DateTime.Now;

    }

    //public class TimesheetSearchViewModel : TimesheetEmployeeSearchViewModel
    //{
    //    public ControlTypeCatalog? ControlType { get; set; }
    //}

    public class TimesheetBriefViewModel : ISelect2BaseVM
    {
        public AssetBriefViewModel Asset { get; set; }
        public CraftSkillBriefViewModel Craft { get; set; }
        public TechnicianBriefViewModel Technician { get; set; }
        public bool IsApproved { get; set; }
        public double STRate { get; set; }
        public double OTRate { get; set; }
        public double DTRate { get; set; }
        public double TotalHours
        {
            get
            {
                var totalST = TimesheetBreakdowns.Select(x => x.STHours).Sum();
                var totalOT = TimesheetBreakdowns.Select(x => x.OTHours).Sum();
                var totalDT = TimesheetBreakdowns.Select(x => x.DTHours).Sum();
                return (totalST + totalOT + totalDT);
            }
        }
        public double TotalSTHours
        {
            get
            {
                return TimesheetBreakdowns.Select(x => x.STHours).Sum();
            }
        }
        public double TotalOTHours
        {
            get
            {
                return TimesheetBreakdowns.Select(x => x.OTHours).Sum();
            }
        }
        public double TotalDTHours
        {
            get
            {
                return TimesheetBreakdowns.Select(x => x.DTHours).Sum();
            }
        }

        public double TotalCost
        {
            get
            {
                var totalST = TimesheetBreakdowns.Select(x => x.STHours).Sum();
                var totalOT = TimesheetBreakdowns.Select(x => x.OTHours).Sum();
                var totalDT = TimesheetBreakdowns.Select(x => x.DTHours).Sum();
                return ((totalST * STRate) + (totalOT * OTRate) + (totalDT * DTRate));
            }
        }
        public long TimeSheetId { get; set; }
        public DateTime? WeekEnding { get; set; }
        public string? Note { get; set; }
        public List<TimesheetBreakdownViewModel> TimesheetBreakdowns { get; set; } = new List<TimesheetBreakdownViewModel>();
        public long? Id { get; set; }

        public string? Select2Text { get; set; }
    }

    public class TimesheetBreakdownViewModel : ITimesheetBreakdownModel
    {
        public long Id { get; set; }
        public bool IsOnSite { get; set; }
        public bool DisableDayEntry { get; set; }
        public bool DisableDayPerDiemEntry { get; set; }
        public DateTime Date { get; set; }
        public DayOfWeek Day { get; set; }

        private double _stHours;
        public double STHours { get { return DisableDayEntry ? 0 : _stHours; } set { _stHours = value; } }

        private double _otHours;
        public double OTHours { get { return DisableDayEntry ? 0 : _otHours; } set { _otHours = value; } }

        private double _dtHours;
        public double DTHours { get { return DisableDayEntry ? 0 : _dtHours; } set { _dtHours = value; } }

        public double STRate { get; set; }
        public double OTRate { get; set; }
        public double DTRate { get; set; }

        private double _totalHours = 0f;
        public double TotalHours { get { return DisableDayEntry ? 0 : _totalHours; } set { _totalHours = value; } }

        public double TotalCost
        {
            get
            {
                return ((STHours * STRate) + (OTHours * OTRate) + (DTHours * DTRate));
            }
        }
    }

    public class TimesheetBreakdownUpdateViewModel
    {
        public long TimeSheetId { get; set; }
        public List<TimesheetBreakdownViewModel> TimesheetBreakdowns { get; set; } = new List<TimesheetBreakdownViewModel>();
    }

    public class TimesheetWebUpdateViewModel : TimesheetBreakdownUpdateViewModel
    {
        public string Note { get; set; }
        public long EmployeeContractId { get; set; }
        public DateTime WeekEnding { get; set; } = DateTime.Now;
        public double TotalHoursByWeek
        {
            get
            {
                if (TimesheetBreakdowns != null && TimesheetBreakdowns.Count > 0)
                {
                    return TimesheetBreakdowns.Sum(x => x.TotalHours);
                }
                return 0;
            }
        }
        public double TotalSTByWeek
        {
            get
            {
                if (TimesheetBreakdowns != null && TimesheetBreakdowns.Count > 0)
                {
                    return TimesheetBreakdowns.Sum(x => x.STHours);
                }
                return 0;
            }
        }
        public double TotalOTByWeek
        {
            get
            {
                if (TimesheetBreakdowns != null && TimesheetBreakdowns.Count > 0)
                {
                    return TimesheetBreakdowns.Sum(x => x.OTHours);
                }
                return 0;
            }
        }
        public double TotalDTByWeek
        {
            get
            {
                if (TimesheetBreakdowns != null && TimesheetBreakdowns.Count > 0)
                {
                    return TimesheetBreakdowns.Sum(x => x.DTHours);
                }
                return 0;
            }
        }
    }


    public class TimesheetProjectsViewModel
    {
        public long TechnicianId { get; set; }
        public List<WorkOrderViewModel> TechniciansWorkOrders { get; set; } = new List<WorkOrderViewModel>();
    }

    public class WorkOrderViewModel
    {
        public TimesheetWorkOrderDetailAPIViewModel WorkOrder { get; set; } = new();
        public List<TechnicianWorkOrderCraft> Craft { get; set; } = new List<TechnicianWorkOrderCraft>();
    }

    public class InputBriefVM
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }

    public class TechnicianWorkOrderCraft
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long WorkOrderTechnician { get; set; }
    }

    public class TimesheetBreakdownForModalVM
    {
        public DayOfWeek Day { get; set; }
        public double TotalHours { get; set; }
    }

    public class TimesheetBreakdownResForModalVM
    {
        public double TotalHoursByWeek { get; set; }
        public double TotalSTByWeek { get; set; }
        public double TotalOTByWeek { get; set; }
        public double TotalDTByWeek { get; set; }
    }
}
