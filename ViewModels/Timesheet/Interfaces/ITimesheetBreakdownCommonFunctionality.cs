using Enums;

namespace ViewModels.Timesheet
{
    public interface ITimesheetBreakdownCommonFunctionality
    {
        List<TimesheetBreakdownDetailViewModel> TimesheetBreakdowns { get; set; }
        double TOTSt { get; set; }
        double TOTOt { get; set; }
        double TOTDt { get; set; }
        double TOTPaidSt { get; set; }
        double TOTPaidOt { get; set; }
        double TOTPaidDt { get; set; }
        double TotalPerDiem { get; set; }
        double TotalPaidPerDiem { get; set; }
        double TotalCost { get; set; }
        string TotalCostFormatted { get; set; }
        double TOTPaidOtherPayments { get; set; }
        double TotalReceivedCost { get; set; }
        string TotalReceivedCostFormatted { get; set; }
        double BalanceDue { get; set; }
        string BalanceDueFormatted { get; set; }
        double Material { get; set; }
        double Equipment { get; set; }
        double Other { get; set; }
        double Airfare { get; set; }
        double Miles { get; set; }
        PaymentIndicatorStatus? PaymentIndicator { get; set; }
        double STRate { get; set; }
        double OTRate { get; set; }
        double DTRate { get; set; }

        public void ProcessBreakdowns(ITimesheetBreakdownCommonFunctionality model);
    }
}