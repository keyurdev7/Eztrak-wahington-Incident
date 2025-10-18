using System.ComponentModel.DataAnnotations.Schema;
using Models.Models.Shared;

namespace Models
{
    public class OrderItem : BaseDBModel
    {
        public long Quantity { get; set; }
        public bool IsIssued { get; set; }

        [ForeignKey("Inventory")]
        public long? InventoryId { get; set; }
        public Inventory? Inventory { get; set; }

        [ForeignKey("Equipment")]
        public long? EquipmentId { get; set; }
        public Equipment? Equipment { get; set; }

        [ForeignKey("Order")]
        public long OrderId { get; set; }
        public Order Order { get; set; }

    }
}

