using System.ComponentModel.DataAnnotations;

namespace ViewModels
{
    public class TaskEquipmentViewModel
    {
        public long Id { get; set; }
        [Required]
        public string? EquipmentName { get; set; }
        public EquipmentBriefViewModel Equipment { get; set; } = new();
        public double Cost { get; set; }
    }
}
