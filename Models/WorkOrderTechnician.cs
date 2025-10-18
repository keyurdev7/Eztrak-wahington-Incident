using Models.Models.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class WorkOrderTechnician : BaseDBModel
    {
        [ForeignKey("Technician")]
        public long TechnicianId { get; set; }
        public ApplicationUser Technician { get; set; }

        [ForeignKey("WorkOrder")]
        public long WorkOrderId { get; set; }
        public WorkOrder WorkOrder { get; set; }

        [ForeignKey("CraftSkill")]
        public long CraftSkillId { get; set; }
        public CraftSkill CraftSkill { get; set; }
    }
}
