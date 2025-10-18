using Enums;
using Models.Models.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class WorkOrderTasks : BaseDBModel
    {
        public string TaskDescription { get; set; }
        public DateTime? TaskDueDate { get; set; }
        public WOTaskStatusCatalog Status { get; set; }

        [ForeignKey("WorkOrder")]
        public long WorkOrderId { get; set; }
        public WorkOrder WorkOrder { get; set; }
    }
}
