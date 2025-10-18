using Models.Models.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class EquipmentNotes : BaseDBModel
    {
        public string Description { get; set; }
        public string? FileUrl { get; set; }

        [ForeignKey("Equipment")]
        public long EquipmentId { get; set; }
        public Equipment Equipment { get; set; }
    }
}
