using Enums;
using Models.Models.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class Asset : BaseDBModel
    {
        public string SystemGeneratedId { get; set; }
        public string? Description { get; set; }
        public string? PoleId { get; set; }
        public string? AssetClass { get; set; }
        public double? Value { get; set; }
        public double? MaintenanceCost { get; set; }
        public string Intersection { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime InstalledDate { get; set; }
        public DateTime? ReplacementDate { get; set; }
        public DateTime? NextMaintenanceDate { get; set; }
        public ReplacementCycleCatalog? ReplacementYear { get; set; }
        public MaintenanceCycleCatalog? NextMaintenanceYear { get; set; }

        [ForeignKey("AssetType")]
        public long? AssetTypeId { get; set; }
        public AssetType? AssetType { get; set; }

        [ForeignKey("Manufacturer")]
        public long? ManufacturerId { get; set; }
        public Manufacturer? Manufacturer { get; set; }

        [ForeignKey("Condition")]
        public long? ConditionId { get; set; }
        public Condition? Condition { get; set; }

        [ForeignKey("MUTCD")]
        public long? MUTCDId { get; set; }
        public MUTCD? MUTCD { get; set; }

        [ForeignKey("MountType")]
        public long? MountTypeId { get; set; }
        public MountType? MountType { get; set; }
    }
}
