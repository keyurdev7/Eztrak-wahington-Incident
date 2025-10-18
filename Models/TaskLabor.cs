using Models.Models.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class TaskLabor : BaseDBModel
    {
        public double Hours { get; set; }
        public double Rate { get; set; }
        public double Total { get; set; }

        [ForeignKey("CraftSkill")]
        public long? CraftSkillId { get; set; }
        public CraftSkill? CraftSkill { get; set; }

        [ForeignKey("TaskType")]
        public long TaskTypeId { get; set; }
        public TaskType TaskType { get; set; }
    }
}
