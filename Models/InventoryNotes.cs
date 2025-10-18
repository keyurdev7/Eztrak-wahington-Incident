using Models.Models.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class InventoryNotes : BaseDBModel
    {
        public string Description { get; set; }
        public string? FileUrl { get; set; }

        [ForeignKey("Inventory")]
        public long InventoryId { get; set; }
        public Inventory Inventory { get; set; }
    }
}
