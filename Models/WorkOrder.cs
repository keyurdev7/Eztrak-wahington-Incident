using Enums;
using Models.Models.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class WorkOrder : BaseDBModel
    {
        public string SystemGeneratedId { get; set; }
        public TaskCatalog Task { get; set; }
        public WOStatusCatalog Status { get; set; }
        public WorkOrderTypeCatalog Type { get; set; }
        public Urgency Urgency { get; set; }
        public double TotalHours { get; set; }
        public double LabourCost { get; set; }
        public double EquipmentCost { get; set; }
        public double MaterialCost { get; set; }
        public double TotalCost { get; set; }
        public double ActualHours { get; set; }
        public double ActualCost { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Intersection { get; set; }

        [ForeignKey("AssetType")]
        public long? AssetTypeId { get; set; }
        public AssetType? AssetType { get; set; }

        [ForeignKey("StreetServiceRequest")]
        public long? StreetServiceRequestId { get; set; }
        public StreetServiceRequest? StreetServiceRequest { get; set; }

        public DateTime? DueDate { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public string? DefaultImageUrl { get; set; }

        [ForeignKey("Asset")]
        public long? AssetId { get; set; }
        public Asset? Asset { get; set; }

        [ForeignKey("ManagerId")]
        public long? ManagerId { get; set; }
        public ApplicationUser? Manager { get; set; }

        [ForeignKey("Repair")]
        public long? RepairId { get; set; }
        public Repair? Repair { get; set; }

        [ForeignKey("Replace")]
        public long? ReplaceId { get; set; }
        public Replace? Replace { get; set; }

        [ForeignKey("TaskType")]
        public long? TaskTypeId { get; set; }
        public TaskType? TaskType { get; set; }

        public List<WorkOrderTechnician>? Technicians { get; set; }
        public List<WorkOrderComment>? WorkOrderComments { get; set; } = new();
    }
}
