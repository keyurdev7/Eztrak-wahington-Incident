using System.ComponentModel.DataAnnotations;

namespace ViewModels
{
    public class TaskMaterialViewModel
    {
        public long? Id { get; set; }
        public InventoryBriefViewModel Material { get; set; } = new();

        [Required]
        public string? MaterialName { get; set; }
        public string? ItemNo { get; set; }
        public double Cost { get; set; }
    }
}
