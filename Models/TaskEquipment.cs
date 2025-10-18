using Enums;
using Models.Models.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class TaskEquipment : BaseDBModel
    {
        public double Cost { get; set; }
        public string? EquipmentName { get; set; }

        [ForeignKey("Equipment")]
        public long? EquipmentId { get; set; }
        public Equipment? Equipment { get; set; }
        
        [ForeignKey("TaskType")]
        public long TaskTypeId { get; set; }
        public TaskType TaskType { get; set; }
    }
}
