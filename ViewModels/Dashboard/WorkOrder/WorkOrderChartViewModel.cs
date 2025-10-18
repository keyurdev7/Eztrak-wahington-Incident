using Enums;
using Helpers.Datetime;
using ViewModels.Charts.Interfaces;

namespace ViewModels.Report.WorkOrder
{

    public class WorkOrderChartViewModel : IChartDataViewModel
    {
        public string Category { get; set; } = "";

        public double Value { get; set; }
        public string FormattedValue
        {
            get { return Value.ToString(); }
        }
    }

    public class WorkOrderDashboardViewModel
    {
        public string Status { get; set; }
        public List<WorkOrderDashboardDataViewModel> WorkOrders { get; set; } = new();
    }

    public class WorkOrderDashboardDataViewModel
    {
        public long Id { get; set; }
        public string Number { get; set; }
        public DateTime? NeedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string FormattedNeedBy { get => NeedBy != null && NeedBy != DateTime.MinValue ? NeedBy.Value.FormatDate() : "-"; }
        public string Manager { get; set; }
        public string ImageUrl { get; set; }
        public Urgency Urgency { get; set; }

        public string Title { get; set; }
        public string WorkStep { get; set; }

        public string FormattedTitle
        {
            get
            {
                return string.IsNullOrEmpty(Title) ? WorkStep : Title;
            }
        }

        public string? Description { get; set; }
    }
}
