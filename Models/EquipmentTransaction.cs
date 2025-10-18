using System.ComponentModel.DataAnnotations.Schema;
using Enums;
using Models.Models.Shared;

namespace Models
{
    public class EquipmentTransaction : BaseDBModel
    {
        public EquipmentStatusCatalog Status { get; set; }
        public string? PoNo { get; set; }
        public double ItemPrice { get; set; }
        public double Quantity { get; set; }
        public double HourlyRate { get; set; }
        public DateTime PurchaseDate { get; set; }
        public double Hours { get; set; }

        public EquipmentTransactionTypeCatalog TransactionType { get; set; }


        [ForeignKey("Condition")]
        public long? ConditionId { get; set; }
        public Condition? Condition { get; set; }

        [ForeignKey("Equipment")]
        public long EquipmentId { get; set; }
        public Equipment Equipment { get; set; }

        [ForeignKey("SupplierId")]
        public long SupplierId { get; set; }
        public Supplier Supplier { get; set; }

        [ForeignKey("LocationId")]
        public long LocationId { get; set; }
        public Location Location { get; set; }

        [ForeignKey("ReferenceId")]
        public long? ReferenceId { get; set; }
        public EquipmentTransaction? Reference { get; set; }

        public long EntityId { get; set; }
        public long EntityDetailId { get; set; }
    }
}

