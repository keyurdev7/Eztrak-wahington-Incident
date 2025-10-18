using ViewModels.Charts.Interfaces;

namespace ViewModels.Report.WorkOrder
{

    public class WorkOrderByManagerChartViewModel : IChartDataViewModel
    {
        public string Category { get; set; } = "";

        public double Value { get; set; }
        public string FormattedValue
        {
            get { return Value.ToString(); }
        }
    }
}
