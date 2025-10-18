using Enums;
using Helpers.Extensions;

namespace ViewModels.Timesheet
{
    public class TimeSheetExcelVM
    {
        public DateTime Date { get => ExcelDate.ToDateTime(); }
        public string ExcelDate { get; set; }
        public string Employee { get; set; }
        public string Customer { get; set; }
        public string PONumber { get; set; }
        public string POLineItem { get; set; }
        public string TransferToPOLineItem { get; set; }
        public string Craft { get; set; }

        public string ExcelTSRefStatus { get; set; }
        public TSRefStatus? TSRefStatus
        {
            get
            {
                if (string.IsNullOrEmpty(ExcelTSRefStatus))
                {
                    return null;
                }
                TSRefStatus xyz;
                Enum.TryParse<TSRefStatus>(ExcelTSRefStatus, true, out xyz);
                return xyz;
            }
        }


        public long TSRefNumber { get; set; }
        public string ExcelPaymentIndicator { get; set; }
        public PaymentIndicatorStatus? PaymentIndicator
        {
            get
            {
                if (string.IsNullOrEmpty(ExcelPaymentIndicator))
                {
                    return null;
                }
                PaymentIndicatorStatus xyz;
                Enum.TryParse(ExcelPaymentIndicator, true, out xyz);
                return xyz;
            }
        }
        public double STHours { get; set; }
        public double OTHours { get; set; }
        public double DTHours { get; set; }
        public double STRate { get; set; }
        public double OTRate { get; set; }
        public double DTRate { get; set; }
        public double PerDiem { get; set; }
        public double Equipment { get; set; }
        public double Other { get; set; }
        public double Total { get; set; }
    }
}
