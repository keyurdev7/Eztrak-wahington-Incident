using Enums;
using Models.Models.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class WorkOrderLabor : BaseDBModel
    {
        public double DU { get; set; }
        public double MN { get; set; }
        public double Rate { get; set; }
        public double Estimate { get; set; }
        public OverrideTypeCatalog? Type { get; set; }

        [ForeignKey("CraftSkill")]
        public long CraftId { get; set; }
        public CraftSkill CraftSkill { get; set; }

        [ForeignKey("WorkOrder")]
        public long WorkOrderId { get; set; }
        public WorkOrder WorkOrder{ get; set; }
    }
}
