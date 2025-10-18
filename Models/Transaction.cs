using Enums;
using Models.Models.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class Transaction : BaseDBModel
    {
        public string? LotNO { get; set; }
        public double Quantity { get; set; }
        public double ItemPrice { get; set; }
        public TransactionTypeCatalog TransactionType { get; set; }

        [ForeignKey("SourceId")]
        public long? SourceId { get; set; }
        public Source? Source { get; set; }

        [ForeignKey("SupplierId")]
        public long SupplierId { get; set; }
        public Supplier Supplier { get; set; }

        [ForeignKey("LocationId")]
        public long LocationId { get; set; }
        public Location Location { get; set; }

        [ForeignKey("InventoryId")]
        public long InventoryId { get; set; }
        public Inventory Inventory { get; set; }

        [ForeignKey("ReferenceId")]
        public long? ReferenceId { get; set; }
        public Transaction? Reference { get; set; }

        public long EntityId { get; set; }
        public long EntityDetailId { get; set; }

    }
}
