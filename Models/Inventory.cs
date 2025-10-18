using Models.Models.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class Inventory : BaseDBModel
    {
        public string SystemGeneratedId { get; set; }
        public string Description { get; set; }
        public string ItemNo { get; set; }
        public double MinimumQuantity { get; set; }

        [ForeignKey("CategoryId")]
        public long CategoryId { get; set; }
        public Category Category { get; set; }

        [ForeignKey("ManufacturerId")]
        public long ManufacturerId { get; set; }
        public Manufacturer Manufacturer { get; set; }

        [ForeignKey("UOMId")]
        public long UOMId { get; set; }
        public UOM UOM { get; set; }

        [ForeignKey("MUTCD")]
        public long? MUTCDId { get; set; }
        public MUTCD? MUTCD { get; set; }
        public string? ImageUrl { get; set; }
    }
}
