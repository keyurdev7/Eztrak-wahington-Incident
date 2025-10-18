using ViewModels.Charts.Interfaces;

namespace ViewModels.Charts
{
    public class ChartDataViewModel: IChartDataViewModel
    {
        public string Category { get; set; } = "-";
        public double Value { get; set; }
        public string FormattedValue { get; set; }
    }
}

