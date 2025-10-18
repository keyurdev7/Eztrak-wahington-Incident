using Enums;
using Helpers.Datetime;
using Helpers.Extensions;
using System.ComponentModel.DataAnnotations;
using ViewModels.WorkOrderTechnician;

namespace ViewModels.Report.RawReport
{
    public class WorkOrderRawReportViewModel
    {
        public long? Id { get; set; }
        public WOStatusCatalog Status { get; set; }
        public string FormattedStatus { get => Status.GetEnumDescription(); }
        public WOSourceStatusCatalog SourceStatus { get; set; }
        public string FormattedSourceStatus { get => SourceStatus.GetEnumDescription(); }
        public DateTime? CreatedOn { get; set; }
        public string FormattedCreatedOn
        {
            get
            {
                return CreatedOn?.FormatDate() ?? "-";
            }
        }
        public string RequestorName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Email { get; set; }
        public string EmailSubject { get; set; }
        public string TypeOfProblem { get; set; }
        [Display(Name = "Street")]
        public string? Intersection { get; set; }
        public string? Description { get; set; }
        [Display(Name = "Work Order #")]
        public string? SystemGeneratedId { get; set; }
        public string Manager { get; set; }
        public Urgency Urgency { get; set; }
        public string FormattedUrgency { get => Urgency.GetEnumDescription(); }
        [Display(Name = "Type", Prompt = "Type")]
        public WorkOrderTypeCatalog Type { get; set; }
        public string FormattedType { get => Type.GetEnumDescription(); }
        public DateTime? DueDate { get; set; }
        public string FormattedDueDate
        {
            get
            {
                return (DueDate != null && DueDate != DateTime.MinValue) ? DueDate.Value.FormatDate() : "-";
            }
        }
        [Display(Name = "Task", Prompt = "Task")]
        public TaskCatalog Task { get; set; }
        public string FormattedTask { get => Task.GetEnumDescription(); }
        public string AssetId { get; set; }
        public string AssetType { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public string FormattedApprovalDate
        {
            get
            {
                return ApprovalDate?.FormatDate() ?? "-";
            }
        }
        public double TotalHours { get; set; }
        public double TotalCost { get; set; }
        public double LabourCost { get; set; }
        public double MaterialCost { get; set; }
        public double EquipmentCost { get; set; }
        public Dictionary<long, WorkOrderInventoryRawData> Materials { get; set; } = new();
        public Dictionary<long, WorkOrderEquipmentRawData> Equipments { get; set; } = new();
        public Dictionary<long, WorkOrderTechnicianRawData> Technicians { get; set; } = new();
    }

    public class WorkOrderInventoryRawData
    {
        public long WorkOrderId { get; }
        public string InventoryName { get; }
        public long InventoryId { get; }
        public double Cost { get; }

        public WorkOrderInventoryRawData(long workOrderId, string inventoryName, long inventoryId, double cost)
        {
            WorkOrderId = workOrderId;
            InventoryName = inventoryName;
            InventoryId = inventoryId;
            Cost = cost;
        }

        public override bool Equals(object? obj)
        {
            return obj is WorkOrderInventoryRawData other &&
                   WorkOrderId == other.WorkOrderId &&
                   InventoryName == other.InventoryName &&
                   InventoryId == other.InventoryId &&
                   Cost == other.Cost;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(WorkOrderId, InventoryName, InventoryId, Cost);
        }
    }

    public class WorkOrderEquipmentRawData
    {
        public long WorkOrderId { get; }
        public string EquipmentName { get; }
        public long EquipmentId { get; }
        public double Cost { get; }

        public WorkOrderEquipmentRawData(long workOrderId, string equipmentName, long equipmentId, double cost)
        {
            WorkOrderId = workOrderId;
            EquipmentName = equipmentName;
            EquipmentId = equipmentId;
            Cost = cost;
        }

        public override bool Equals(object? obj)
        {
            return obj is WorkOrderEquipmentRawData other &&
                   WorkOrderId == other.WorkOrderId &&
                   EquipmentName == other.EquipmentName &&
                   EquipmentId == other.EquipmentId &&
                   Cost == other.Cost;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(WorkOrderId, EquipmentName, EquipmentId, Cost);
        }
    }

    public class WorkOrderTechnicianRawData
    {
        public long WorkOrderId { get; }
        public long TechnicianId { get; }
        public string Name { get; }
        public string? Craft { get; }
        public double Cost { get; }

        public WorkOrderTechnicianRawData(long workOrderId, long technicianId, string name, string? craft, double cost)
        {
            WorkOrderId = workOrderId;
            TechnicianId = technicianId;
            Name = name;
            Craft = craft;
            Cost = cost;
        }

        public override bool Equals(object? obj)
        {
            return obj is WorkOrderTechnicianRawData other &&
                   WorkOrderId == other.WorkOrderId &&
                   TechnicianId == other.TechnicianId &&
                   Name == other.Name &&
                   Craft == other.Craft &&
                   Cost == other.Cost;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(WorkOrderId, TechnicianId, Name, Craft, Cost);
        }
    }
}

