using Enums;
using Pagination;
using ViewModels.CRUD.Interfaces;
using ViewModels.Timesheet.Interfaces;
using ViewModels.Users;
using Web.Extensions;

namespace ViewModels.Timesheet
{
    public class TimesheetBreakdownDetailViewModel : ITimesheetBreakdownModel
    {
        public long Id { get; set; }
        public long TimesheetId { get; set; }
        public DateTime Date { get; set; }
        public bool IsOnSite { get; set; }

        public string FormattedDate
        {
            get
            {
                return Date.Date.ToString("MM/dd/yyyy");
            }
        }

        public DayOfWeek Day { get; set; }

        public float MaxHours { get; set; } = 24;

        public bool DisableDayEntry { get; set; }

        //public bool DisableDayPerDiemEntry { get; set; }

        //private double _dailyPerDiem;
        //public double DailyPerDiem
        //{
        //    get
        //    {
        //        return (DisableDayPerDiemEntry || !IncludePerDiem) ? 0 : _dailyPerDiem;
        //    }
        //    set
        //    {
        //        _dailyPerDiem = value;
        //    }
        //}

        //private bool _includePerDiem = true;
        //public bool IncludePerDiem { get { return DisableDayPerDiemEntry ? false : _includePerDiem; } set { _includePerDiem = value; } }

        private double _regularHours;
        public double RegularHours { get { return DisableDayEntry ? 0 : _regularHours; } set { _regularHours = value; } }

        private double _overtimeHours;
        public double OvertimeHours { get { return DisableDayEntry ? 0 : _overtimeHours; } set { _overtimeHours = value; } }

        private double _doubletimeHours;
        public double DoubleTimeHours { get { return DisableDayEntry ? 0 : _doubletimeHours; } set { _doubletimeHours = value; } }

        //public TSRefStatus? TSRefStatus { get; set; }

        //public string FormattedTSRefStatus
        //{
        //    get
        //    {
        //        if (TSRefStatus == null)
        //            return "";
        //        var fieldInfo = typeof(TSRefStatus).GetField(this.TSRefStatus.ToString());
        //        var descriptionAttribute = fieldInfo.GetCustomAttribute<DescriptionAttribute>();

        //        return descriptionAttribute?.Description ?? this.TSRefStatus.ToString();
        //    }
        //}
        //public float TSRefNumber { get; set; }
        //public PaymentIndicatorStatus? PaymentIndicator { get; set; }

    }

    public class TimesheetBreakdownDetailForReportViewModel : TimesheetBreakdownDetailViewModel
    {
        public TimesheetReportViewModel Timesheet { get; set; } = new TimesheetReportViewModel();
        public string LocationSite { get; set; }
        public double Total
        {
            get
            {
                var STAmount = (RegularHours * Timesheet.CraftSkill.STRate).RoundDouble();
                var DTAmount = (DoubleTimeHours * Timesheet.CraftSkill.DTRate).RoundDouble();
                var OTAmount = (OvertimeHours * Timesheet.CraftSkill.OTRate).RoundDouble();
                var sum = (STAmount + DTAmount + OTAmount + Timesheet.PerDiem + Timesheet.Equipment + Timesheet.Other).RoundDouble();
                return sum;
                //return 0;
            }
        }
        public string POLineItem { get; set; }
        public string TotalFormatted { get => Total.ToString("C"); }
        //public double TotalReceivedCost { get => PaymentIndicator == PaymentIndicatorStatus.Paid ? Total : 0; }
        //public string TotalReceivedCostFormatted { get => TotalReceivedCost.ToString("C"); }
        //public double BalanceDue { get => PaymentIndicator == PaymentIndicatorStatus.Unpaid ? Total : 0; }
        //public string BalanceDueFormatted { get => BalanceDue.ToString("C"); }
    }

    public class TimeSheetBreakdownReportSearchViewModel : BaseSearchModel, ISaveSearch
    {
        public TechnicianBriefViewModel Technician { get; set; } = new();
        public UserBriefViewModel Approver { get; set; } = new();
        public CraftSkillBriefViewModel Craft { get; set; } = new();
        public WorkOrderBriefViewModel WorkOrder { get; set; } = new();
        //  public string? PONumber { get; set; }
        public long? Day { get; set; }

        public DateTime From { get; set; } = DateTime.Today.AddDays(-20);
        public DateTime To { get; set; } = DateTime.Today.AddDays(20).AddMilliseconds(-1);

        public string FormattedDateFilter
        {
            get => From != DateTime.MinValue && To != DateTime.MinValue
               ? $"{From:MM/dd/yyyy} - {To:MM/dd/yyyy}"
               : string.Empty;
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    var dates = value.Split(" - ");
                    if (dates.Length == 2 &&
                        DateTime.TryParse(dates[0], out DateTime startDate) &&
                        DateTime.TryParse(dates[1], out DateTime endDate))
                    {
                        From = startDate;
                        To = endDate;
                    }
                }
            }
        }
        public UserSearchSettingBriefViewModel UserSearchSetting { get; set; } = new();
        public SearchFilterTypeCatalog? Type { get; set; }
        public string SearchView { get; set; }
        //public List<string> PONumberIds { get; set; } = new List<string>();
        //public List<BaseBriefVM> PONumbers { get; set; } = new List<BaseBriefVM>();
        //[DisplayName("TS Ref Status")]
        //public TSRefStatus? TSRefStatus { get; set; }
        //[DisplayName("Payment Indicator")]
        //public PaymentIndicatorStatus? PaymentIndicator { get; set; }
        //[DisplayName("TS Ref Number")]
        //public float? TSRefNumber { get; set; }

        //[DisplayName("Contract Status")]
        //public ActiveStatus? ContractStatus { get; set; }

        //[DisplayName("Contract Item Status")]
        //public ActiveStatus? ContractItemStatus { get; set; }
        //[DisplayName("PO Line Item")]
        //public string? POLineItem { get; set; }
    }

}
