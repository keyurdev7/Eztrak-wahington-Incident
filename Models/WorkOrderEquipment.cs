using Enums;
using Models.Models.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class WorkOrderEquipment : BaseDBModel
    {
        public long Quantity { get; set; }
        public double Hours { get; set; }

        [ForeignKey("Equipment")]
        public long EquipmentId { get; set; }
        public Equipment Equipment { get; set; }

        [ForeignKey("WorkOrder")]
        public long WorkOrderId { get; set; }
        public WorkOrder WorkOrder { get; set; }
    }
}
