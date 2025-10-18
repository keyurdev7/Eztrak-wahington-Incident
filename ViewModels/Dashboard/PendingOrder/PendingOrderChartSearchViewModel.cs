using Helpers.Reports;
using ViewModels.Charts.Interfaces;

namespace ViewModels.Report.PendingOrder
{
    public class PendingOrderChartSearchViewModel : PendingOrderReportSearchViewModel, IChartBaseSearchModel
    {
        public PendingOrderChartSearchViewModel() : base(TimePeriodGroupingType.Weekly, DateTime.Now.AddMonths(-1), DateTime.Now.Date.AddDays(1).AddSeconds(-1))
        {

        }
        public bool LoadSavedFilters { get; set; }
    }
}
