using System.ComponentModel.DataAnnotations.Schema;
using Enums;
using Models.Models.Shared;

namespace Models
{
    public class Order : BaseDBModel
    {
        public string? Notes { get; set; }
        public OrderStatus Status { get; set; }
        public string OrderNumber { get; set; }
        public OrderTypeCatalog Type { get; set; }
        public double? Cost { get; set; }

        [ForeignKey("WorkOrder")]
        public long WorkOrderId { get; set; }
        public WorkOrder WorkOrder { get; set; }

        public List<OrderItem> OrderItems { get; set; }
    }
}

