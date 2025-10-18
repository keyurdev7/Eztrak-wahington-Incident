using Enums;
using Helpers.Reports;
using System.ComponentModel.DataAnnotations;
using ViewModels.Dashboard;
using ViewModels.Dashboard.interfaces;
using ViewModels.Manager;

namespace ViewModels.Report.PendingOrder
{
    public class PendingOrderReportSearchViewModel : DashboardSearchViewModel, ITimePeriodGroupingType
    {
        public PendingOrderReportSearchViewModel()
        {

        }
        public PendingOrderReportSearchViewModel(TimePeriodGroupingType timePeriodGroupingType, DateTime fromDate, DateTime toDate)
        {
            TimePeriodGroupingType = timePeriodGroupingType;
            FromDate = fromDate;
            ToDate = toDate;
        }
        public ManagerBriefViewModel? Manager { get; set; } = new();
        public ManufacturerBriefViewModel? Manufacturer { get; set; } = new();
        [Display(Name = "Grouping Period")]
        public TimePeriodGroupingType TimePeriodGroupingType { get; set; }
        [Display(Name = "Grouping Entity")]
        public PendingOrderReportEntityGroupingType PendingOrderReportEntityGroupingType { get; set; }
    }
}
