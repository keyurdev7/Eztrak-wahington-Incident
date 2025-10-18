using ViewModels.Charts.Interfaces;

namespace ViewModels.Report.PendingOrder
{

    public class PendingOrderChartViewModel : PendingOrderReportViewModel, IChartDataViewModel
    {
        public string Category
        {
            get
            {
                return Label;
            }
        }

        public double Value
        {
            get
            {
                return Orders;
            }
        }
        public string FormattedValue
        {
            get { return Value.ToString(); }
        }
    }
}
