using System.ComponentModel.DataAnnotations;
using Enums;
using Helpers.Datetime;
using ViewModels.Shared;
using ViewModels.Users;

namespace ViewModels
{
    public class OrderDetailViewModel : BaseCrudViewModel
    {
        public long Id { get; set; }

        public OrderTypeCatalog Type { get; set; }

        public OrderStatus Status { get; set; }

        public string? Notes { get; set; }

        public string OrderNumber { get; set; }

        public double TotalCost { get; set; }

        public DateTime CreatedOn { get; set; }

        public string FormattedCreatedOn
        {
            get
            {
                return CreatedOn.FormatDateInPST();
            }
        }

        public UserBriefViewModel Requestor { get; set; } = new();

        public WorkOrderDetailViewModel WorkOrder { get; set; } = new();

        public List<OrderItemDetailViewModel> OrderItems { get; set; } = new();
    }
}
