using ViewModels.Charts.Interfaces;

namespace ViewModels.Report.AssetsByCondition
{

    public class ChartViewModel : IChartDataViewModel
    {
        public string Category { get; set; } = "";

        public double Value { get; set; }
        public string FormattedValue
        {
            get { return Value.ToString(); }
        }
    }
}
