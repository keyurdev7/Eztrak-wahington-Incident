using Models.Models.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class WorkOrderNotes : BaseDBModel
    {
        public string Description { get; set; }
        public string? FileUrl { get; set; }

        [ForeignKey("WorkOrder")]
        public long WorkOrderId { get; set; }
        public WorkOrder WorkOrder { get; set; }
    }
}
