using Enums;
using Microsoft.AspNetCore.Http;
using ViewModels.Manager;
using ViewModels.Shared;
using ViewModels.WorkOrder;
using ViewModels.WorkOrderTechnician;

namespace ViewModels
{
    public interface IWorkOrderModifyViewModel
    {
        string? SystemGeneratedId { get; set; }
        WOAssetViewModel Asset { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        WOStatusCatalog Status { get; set; }
        TaskCatalog? Task { get; set; }
        WorkOrderTypeCatalog Type { get; set; }
        Urgency? Urgency { get; set; }
        double TotalHours { get; }
        double TotalLaborsCost { get; }
        double TotalMaterialCost { get; set; }
        double TotalCost { get; }
        double ActualHours { get; set; }
        double ActualCost { get; set; }
        public long? StreetServiceRequestId { get; set; }
        ManagerBriefViewModel Manager { get; set; }
        RepairBriefViewModel Repair { get; set; }
        ReplaceBriefViewModel Replace { get; set; }
        List<WorkOrderLaborModifyViewModel> WorkOrderLabors { get; set; }
        List<WorkOrderMaterialModifyViewModel> WorkOrderMaterials { get; set; }
        List<WorkOrderEquipmentModifyViewModel> WorkOrderEquipments { get; set; }
        List<WorkOrderTechnicianModifyViewModel> WorkOrderTechnicians { get; set; }
        List<AttachmentVM> ImagesList { get; set; }
        List<IFormFile> Images { get; set; }

        public string? Intersection { get; set; }

        public AssetTypeBriefViewModel? AssetType { get; set; }
    }
}