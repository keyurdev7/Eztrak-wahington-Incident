using Models.Models.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class Equipment : BaseDBModel
    {
        public string SystemGeneratedId { get; set; }
        public string ItemNo { get; set; }

        public double HourlyRate { get; set; }

        public string? EquipmentModel { get; set; }
        public string? Description { get; set; }
        public double TotalValue { get; set; }

        [ForeignKey("CategoryId")]
        public long CategoryId { get; set; }
        public Category Category { get; set; }

        [ForeignKey("ManufacturerId")]
        public long ManufacturerId { get; set; }
        public Manufacturer Manufacturer { get; set; }

        [ForeignKey("UOMId")]
        public long UOMId { get; set; }
        public UOM UOM { get; set; }

        public string? ImageUrl { get; set; }
        public List<EquipmentTransaction> EquipmentTransactions { get; set; }
    }
}
