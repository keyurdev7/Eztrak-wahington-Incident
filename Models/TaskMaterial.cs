using Enums;
using Models.Models.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class TaskMaterial : BaseDBModel
    {
        public double Cost { get; set; }
        public string? MaterialName { get; set; }

        [ForeignKey("Material")]
        public long? MaterialId { get; set; }
        public Inventory? Material { get; set; }

        [ForeignKey("TaskType")]
        public long TaskTypeId { get; set; }
        public TaskType TaskType { get; set; }
    }
}
