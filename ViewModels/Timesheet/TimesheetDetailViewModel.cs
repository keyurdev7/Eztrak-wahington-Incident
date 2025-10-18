using ViewModels.Shared;
using ViewModels.Employee;
using Enums;
using ViewModels.Technician;
using ViewModels.Manager;
using ViewModels.CRUD.Interfaces;

namespace ViewModels.Timesheet
{
    public class TimesheetListViewModel : BaseCrudViewModel, ITimesheetBreakdownCommonFunctionality, ILastUpdatedBy
    {
        public long Id { get; set; }
        public long WorkOrderTechnicianId { get; set; }
        public DateTime WeekEnding { get; set; }
        public DateTime DueDate { get; set; }
        public double STHours { get; set; }
        public double OTHours { get; set; }
        public double DTHours { get; set; }

        public string FormattedWeekEnding
        {
            get
            {
                return WeekEnding.Date.ToString("MM/dd/yyyy");
            }
        }
        public double MonSt
        {
            get
            {
                if (TimesheetBreakdowns != null && TimesheetBreakdowns.Count > 0)
                {
                    var hours = TimesheetBreakdowns.Where(x => x.Day == DayOfWeek.Monday).Select(x => x.RegularHours + x.OvertimeHours + x.DoubleTimeHours).FirstOrDefault();
                    return hours;
                }
                return 0;
            }
        }
        public double TueSt
        {
            get
            {
                if (TimesheetBreakdowns != null && TimesheetBreakdowns.Count > 0)
                {
                    var hours = TimesheetBreakdowns.Where(x => x.Day == DayOfWeek.Tuesday).Select(x => x.RegularHours + x.OvertimeHours + x.DoubleTimeHours).FirstOrDefault();
                    return hours;
                }
                return 0;
            }
        }
        public double WedSt
        {
            get
            {
                if (TimesheetBreakdowns != null && TimesheetBreakdowns.Count > 0)
                {
                    var hours = TimesheetBreakdowns.Where(x => x.Day == DayOfWeek.Wednesday).Select(x => x.RegularHours + x.OvertimeHours + x.DoubleTimeHours).FirstOrDefault();
                    return hours;
                }
                return 0;
            }
        }
        public double ThurSt
        {
            get
            {
                if (TimesheetBreakdowns != null && TimesheetBreakdowns.Count > 0)
                {
                    var hours = TimesheetBreakdowns.Where(x => x.Day == DayOfWeek.Thursday).Select(x => x.RegularHours + x.OvertimeHours + x.DoubleTimeHours).FirstOrDefault();
                    return hours;
                }
                return 0;
            }
        }
        public double FriSt
        {
            get
            {
                if (TimesheetBreakdowns != null && TimesheetBreakdowns.Count > 0)
                {
                    var hours = TimesheetBreakdowns.Where(x => x.Day == DayOfWeek.Friday).Select(x => x.RegularHours + x.OvertimeHours + x.DoubleTimeHours).FirstOrDefault();
                    return hours;
                }
                return 0;
            }
        }
        public double SatSt
        {
            get
            {
                if (TimesheetBreakdowns != null && TimesheetBreakdowns.Count > 0)
                {
                    var hours = TimesheetBreakdowns.Where(x => x.Day == DayOfWeek.Saturday).Select(x => x.RegularHours + x.OvertimeHours + x.DoubleTimeHours).FirstOrDefault();
                    return hours;
                }
                return 0;
            }
        }
        public double SunSt
        {
            get
            {
                if (TimesheetBreakdowns != null && TimesheetBreakdowns.Count > 0)
                {
                    var hours = TimesheetBreakdowns.Where(x => x.Day == DayOfWeek.Sunday).Select(x => x.RegularHours + x.OvertimeHours + x.DoubleTimeHours).FirstOrDefault();
                    return hours;
                }
                return 0;
            }
        }
        public List<TimesheetBreakdownDetailViewModel> TimesheetBreakdowns { get; set; } = new List<TimesheetBreakdownDetailViewModel>();
        public string? LastUpdatedBy { get; set; }
        public DateTime LastUpdatedOn { get; set; }
        public double TOTPaidSt { get; set; }
        public double TOTPaidOt { get; set; }
        public double TOTPaidDt { get; set; }
        public double TotalPerDiem { get; set; }
        public double DailyPerDiem { get; set; }
        public double TotalPaidPerDiem { get; set; }
        public double TOTPaidOtherPayments { get; set; }
        public double TotalCost { get; set; }
        public string TotalCostFormatted { get; set; }
        public double TotalReceivedCost { get; set; }
        public string TotalReceivedCostFormatted { get; set; }
        public double BalanceDue { get; set; }
        public string BalanceDueFormatted { get; set; }
        public double Material { get; set; }
        public double Equipment { get; set; }
        public double Other { get; set; }
        public double Airfare { get; set; }
        public double Miles { get; set; }
        public PaymentIndicatorStatus? PaymentIndicator { get; set; }
        public double STRate { get; set; }
        public double OTRate { get; set; }
        public double DTRate { get; set; }
        public double TOTSt { get; set; }
        public double TOTOt { get; set; }
        public double TOTDt { get; set; }

        public void ProcessBreakdowns(ITimesheetBreakdownCommonFunctionality model)
        {
            new TimesheetBreakdownCommonFunctionality().ProcessBreakdowns(this);
        }

        public WorkOrderBriefViewModel WorkOrder { get; set; }
    }

    public class TimesheetDetailViewModel : TimesheetListViewModel, ILastUpdatedBy
    {
        public EmployeeBriefViewModel Technician { get; set; } = new EmployeeBriefViewModel();
        public BaseBriefVM Approver { get; set; } = new BaseBriefVM();
        public BaseBriefVM WorkOrder { get; set; } = new BaseBriefVM();
        public string InvoiceNo { get; set; }
        public TimesheetApproveStatus ApproveStatus { get; set; }
        public string? LastUpdatedBy { get; set; }
        public DateTime LastUpdatedOn { get; set; }
        public bool CanApproveTimesheet { get => ApproveStatus == TimesheetApproveStatus.UnApproved; }
    }

    public class TimeSheetWithBreakdownViewModel : ITimesheetBreakdownCommonFunctionality, IBaseCrudViewModel, ILastUpdatedBy
    {
        public long Id { get; set; }
        public ApproveStatus ApproveStatus { get; set; }
        public string? InvoiceNo { get; set; }
        public DateTime WeekEnding { get; set; } = DateTime.Now;
        public DateTime DueDate { get; set; }
        public TechnicianDetailViewModel Technician { get; set; } = new TechnicianDetailViewModel();
        public ManagerDetailViewModel SESApprover { get; set; } = new ManagerDetailViewModel();
        public ManagerDetailViewModel Approver { get; set; } = new ManagerDetailViewModel();
        public WorkOrderDetailViewModel WorkOrder { get; set; } = new WorkOrderDetailViewModel();
        public CraftSkillDetailViewModel Craft { get; set; } = new CraftSkillDetailViewModel();

        public double PerDiem { get; set; }

        public double DailyPerDiem { get; set; }

        public string? Note { get; set; }

        public string FormattedInvoiceNumber
        {
            get
            {
                return InvoiceNo;
                //var poNumber = (Contract?.PONumber) ?? "";
                //if (poNumber.Length < 4)
                //{
                //    poNumber = poNumber.PadLeft(4, '0');
                //}
                //else
                //{
                //    poNumber = poNumber.Substring(poNumber.Length - 4, 4);
                //}
                //var invoiceNo = InvoiceNo.ToString().PadLeft(3, '0');
                //return $"{poNumber} - {invoiceNo}";
            }
        }
        public ActiveStatus ActiveStatus { get; set; }

        public string? LastUpdatedBy { get; set; }
        public DateTime LastUpdatedOn { get; set; }
        public List<TimesheetBreakdownDetailViewModel> TimesheetBreakdowns { get; set; }
        public double TOTSt { get; set; }
        public double TOTOt { get; set; }
        public double TOTDt { get; set; }
        public double TOTPaidSt { get; set; }
        public double TOTPaidOt { get; set; }
        public double TOTPaidDt { get; set; }
        public double TotalPerDiem { get; set; }
        public double TotalPaidPerDiem { get; set; }
        public double TotalCost { get; set; }
        public string TotalCostFormatted { get; set; }
        public double TOTPaidOtherPayments { get; set; }
        public double TotalReceivedCost { get; set; }
        public string TotalReceivedCostFormatted { get; set; }
        public double BalanceDue { get; set; }
        public string BalanceDueFormatted { get; set; }
        public double Material { get; set; }
        public double Equipment { get; set; }
        public double Other { get; set; }
        public double Airfare { get; set; }
        public double Miles { get; set; }
        public PaymentIndicatorStatus? PaymentIndicator { get; set; }
        public double STRate { get; set; }
        public double OTRate { get; set; }
        public double DTRate { get; set; }

        public void ProcessBreakdowns(ITimesheetBreakdownCommonFunctionality model)
        {
            new TimesheetBreakdownCommonFunctionality().ProcessBreakdowns(this);
        }
    }

    public class TimesheetBreakdownCommonFunctionality
    {
        public void ProcessBreakdowns(ITimesheetBreakdownCommonFunctionality model)
        {
            if (model.TimesheetBreakdowns != null && model.TimesheetBreakdowns.Count > 0)
            {
                model.TOTSt = Math.Round(model.TimesheetBreakdowns.Select(x => x.RegularHours).Sum(), 2);
                model.TOTOt = Math.Round(model.TimesheetBreakdowns.Select(x => x.OvertimeHours).Sum(), 2);
                model.TOTDt = Math.Round(model.TimesheetBreakdowns.Select(x => x.DoubleTimeHours).Sum(), 2);
                //model.TOTPaidSt = model.TimesheetBreakdowns.Where(x => x.PaymentIndicator == PaymentIndicatorStatus.Paid).Select(x => x.RegularHours).Sum();
                //model.TOTPaidOt = model.TimesheetBreakdowns.Where(x => x.PaymentIndicator == PaymentIndicatorStatus.Paid).Select(x => x.OvertimeHours).Sum();
                //model.TOTPaidDt = model.TimesheetBreakdowns.Where(x => x.PaymentIndicator == PaymentIndicatorStatus.Paid).Select(x => x.DoubleTimeHours).Sum();
                //model.TotalPerDiem = model.TimesheetBreakdowns.Select(x => Math.Round(x.DailyPerDiem, 2)).Sum();
                //model.TotalPaidPerDiem = model.TimesheetBreakdowns.Where(x => x.PaymentIndicator == PaymentIndicatorStatus.Paid).Select(x => Math.Round(x.DailyPerDiem, 2)).Sum();
            }
            else
            {
                model.TOTSt = 0;
                model.TOTOt = 0;
                model.TOTDt = 0;
                model.TOTPaidSt = 0;
                model.TOTPaidOt = 0;
                model.TOTPaidDt = 0;
                model.TotalPerDiem = 0;
                model.TotalPaidPerDiem = 0;
            }

            model.TotalCost = Math.Round((model.TOTSt * Math.Round(model.STRate, 2)), 2)
                            + Math.Round((model.TOTOt * Math.Round(model.OTRate, 2)), 2)
                            + Math.Round((model.TOTDt * Math.Round(model.DTRate, 2)), 2)
                            + model.TotalPerDiem
                            + model.Material
                            + model.Equipment
                            + model.Airfare
                            + model.Miles
                            + model.Other;

            model.TotalCostFormatted = model.TotalCost.ToString("C");

            if (model.PaymentIndicator == PaymentIndicatorStatus.Paid)
            {
                model.TOTPaidOtherPayments = model.Material + model.Equipment + model.Airfare + model.Miles + model.Other;
            }
            else
                model.TOTPaidOtherPayments = 0;

            model.TotalReceivedCost = Math.Round((model.TOTPaidSt * Math.Round(model.STRate, 2)), 2)
                + Math.Round((model.TOTPaidOt * Math.Round(model.OTRate, 2)), 2)
                + Math.Round((model.TOTPaidDt * Math.Round(model.DTRate, 2)), 2)
                + model.TotalPaidPerDiem
                + model.TOTPaidOtherPayments;

            model.TotalReceivedCostFormatted = model.TotalReceivedCost.ToString("C");
            model.BalanceDue = model.TotalCost - model.TotalReceivedCost;

            model.BalanceDueFormatted = model.BalanceDue.ToString("C");
        }
    }

    public class TimesheetDetailForBreakdownViewModel : TimesheetListViewModel
    {
        public EmployeeDetailViewModel Employee { get; set; } = new EmployeeDetailViewModel();
        public ManagerDetailViewModel Approver { get; set; } = new ManagerDetailViewModel();
        public WorkOrderDetailViewModel WorkOrder { get; set; } = new WorkOrderDetailViewModel();
        public float Material { get; set; }
        public float PerDiem { get; set; }
        public float Equipment { get; set; }
        public float Other { get; set; }
        public float Airfare { get; set; }
        public float Miles { get; set; }
    }
    public class TimesheetReportViewModel
    {
        public TechnicianDetailViewModel Technician { get; set; } = new TechnicianDetailViewModel();
        public ManagerDetailViewModel Approver { get; set; } = new ManagerDetailViewModel();
        public WorkOrderDetailViewModel WorkOrder { get; set; } = new WorkOrderDetailViewModel();
        public CraftSkillBriefViewModel CraftSkill { get; set; } = new();
        public float Material { get; set; }
        public float PerDiem { get; set; }
        public float Equipment { get; set; }
        public float Other { get; set; }
        public float Airfare { get; set; }
        public float Miles { get; set; }
        public string InvoiceNo { get; set; }
    }


    public class TimesheetUpdateViewModel
    {
        public long Id { get; set; }
        public float Material { get; set; }
        public float PerDiem { get; set; }
        public float Equipment { get; set; }
        public float Other { get; set; }
        public float Airfare { get; set; }
        public float Miles { get; set; }
        public ApproveStatus ApproveStatus { get; set; }
    }

    public class TimesheetMappingViewModel : TimesheetListViewModel
    {
        //public EmployeeContractVM WorkOrderTechnician { get; set; }
        public ApproveStatus ApproveStatus { get; set; }

    }

}
