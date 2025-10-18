using Enums;
using Models.Models.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class TaskTypeStep : BaseDBModel
    {
        public string JobType { get; set; }
        public double Men { get; set; }
        public double Hours { get; set; }
        public double Material { get; set; }
        public double Equipment { get; set; }

        [ForeignKey("CraftSkill")]
        public long CraftId { get; set; }
        public CraftSkill CraftSkill { get; set; }

        [ForeignKey("TaskType")]
        public long TaskTypeId { get; set; }
        public TaskType TaskType{ get; set; }
    }
}
