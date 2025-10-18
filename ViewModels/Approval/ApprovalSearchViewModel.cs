using Enums;
using Pagination;

namespace ViewModels
{
    public class ApprovalSearchViewModel : BaseSearchModel
    {
        public ApprovalSearchViewModel()
        {
            OrderByColumn = "WeekEnding";
            OrderDir = PaginationOrderCatalog.Desc;
        }
        public long WorkOrderTechniciantId { get; set; }
        public DateTime? WeekEnding { get; set; }
        public TechnicianBriefViewModel Technician { get; set; } = new();
        public WorkOrderBriefViewModel WorkOrder { get; set; } = new();
        public TimesheetApproveStatus? ApproveStatus { get; set; }
        public long? TimesheetId { get; set; }
        public string PayPeriodRange { get; set; }
        public DateTime? PayPeriodToDate
        {
            get
            {
                if (!string.IsNullOrEmpty(PayPeriodRange))
                {
                    var toDate = DateTime.Parse(PayPeriodRange.Split("-")[1]);
                    return toDate;
                }
                return null;
            }
            set { }

        }

        public DateTime? PayPeriodFromDate
        {
            get
            {
                if (!string.IsNullOrEmpty(PayPeriodRange))
                {
                    var fromDate = DateTime.Parse(PayPeriodRange.Split("-")[0]);
                    return fromDate;
                }
                return null;
            }
            set { }

        }


    }

}
