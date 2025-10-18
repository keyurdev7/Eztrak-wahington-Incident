using Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels.Asset
{
    public class AssetViewModel
    {
        public long AssetPkId { get; set; }
        public string SystemGeneratedId { get; set; }
        public long AssetTypeId { get; set; }
        public string AssetTypeName { get; set; }
        public long ConditionId { get; set; }
        public string ConditionName { get; set; }
        public double MaintenanceCost { get; set; }
        public MaintenanceCycleCatalog? NextMaintenanceYear { get; set; }
        public string Description { get; set; }
        public DateTime InstalledDate { get; set; }
        public string Intersection { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public long ManufacturerId { get; set; }
        public string ManufacturerName { get; set; }
        public long MUTCDId { get; set; }
        public string MUTCDCode { get; set; }
        public string MUTCDImageUrl { get; set; }
        public ReplacementCycleCatalog? ReplacementYear { get; set; }
        public double Value { get; set; }
        public DateTime ReplacementDate { get; set; }
        public DateTime NextMaintenanceDate { get; set; }
        public string? AssetClass { get; set; }
        public string? PoleId { get; set; }
        public long AssetTypeLevel2Id { get; set; }
    }
}
