using Enums;
using Models.Models.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class WorkOrderMaterial : BaseDBModel
    {
        public long Quantity { get; set; }

        [ForeignKey("Inventory")]
        public long InventoryId { get; set; }
        public Inventory Inventory { get; set; }

        [ForeignKey("WorkOrder")]
        public long WorkOrderId { get; set; }
        public WorkOrder WorkOrder{ get; set; }
    }
}
