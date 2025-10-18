using System.ComponentModel.DataAnnotations;
using Models.Common.Interfaces;
using ViewModels.Shared;
using Enums;
using ViewModels.Manager;
using Microsoft.AspNetCore.Http;
using ViewModels.WorkOrderTechnician;
using ViewModels.WorkOrder;
using Helpers.Datetime;
using Helpers.File;
using Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace ViewModels
{
    public class WorkOrderModifyViewModel : BaseUpdateVM, IBaseCrudViewModel, IIdentitifier, IWorkOrderModifyViewModel, IDynamicColumns, IFileModel
    {
        //public string? Name { get; set; }
        [DisplayName("ID #")]

        public string? SystemGeneratedId { get; set; }
        public WOAssetViewModel Asset { get; set; } = new();

        [Required]
        public WOStatusCatalog Status { get; set; }
        [Required]

        [Display(Name = "Job Type", Prompt = "Task")]
        public TaskCatalog? Task { get; set; }
        [Display(Name = "Title", Prompt = "Title")]
        public string? Title { get; set; }
        [Display(Name = "Description", Prompt = "Description")]
        public string? Description { get; set; }
        [Display(Name = "Needed By")]

        public DateTime? DueDate { get; set; }
        [Display(Name = "Completion Date")]

        public DateTime ApprovalDate { get; set; }
        public string FormattedDueDate
        {
            get
            {
                return DueDate?.FormatDate() ?? "-";
            }
        }
        public string FormattedApprovalDate
        {
            get
            {
                return ApprovalDate.FormatDate();
            }
        }
        [Display(Name = "Street", Prompt = "Street")]
        public string? Intersection { get; set; }

        public AssetTypeBriefViewModel? AssetType { get; set; } = new(false, "");
        public long? StreetServiceRequestId { get; set; }

        [Display(Name = "Type", Prompt = "Type")]
        public WorkOrderTypeCatalog Type { get; set; }
        [Required]
        public Urgency? Urgency { get; set; }
        public double TotalHours { get; set; }
        public double TotalLaborsCost { get => WorkOrderLabors != null && WorkOrderLabors.Count > 0 ? WorkOrderLabors.Sum(x => x.LaborEstimate) : 0; }
        public double TotalMaterialCost { get; set; }

        public double LabourCost { get; set; }
        public double EquipmentCost { get; set; }
        public double MaterialCost { get; set; }

        public double TotalCost { get; set; }
        public double ActualHours { get; set; }
        public double ActualCost { get; set; }
        [Display(Name = "Responsible")]
        public ManagerBriefViewModel Manager { get; set; } = new();
        public TaskTypeBriefViewModel TaskType { get; set; } = new();
        public RepairBriefViewModel Repair { get; set; } = new();
        public ReplaceBriefViewModel Replace { get; set; } = new();
        public List<WorkOrderLaborModifyViewModel> WorkOrderLabors { get; set; } = new();
        public List<WorkOrderMaterialModifyViewModel> WorkOrderMaterials { get; set; } = new();
        public List<WorkOrderEquipmentModifyViewModel> WorkOrderEquipments { get; set; } = new();
        public List<WorkOrderTechnicianModifyViewModel> WorkOrderTechnicians { get; set; } = new();
        public List<AttachmentVM> ImagesList { get; set; } = new();
        public List<IFormFile> Images { get; set; } = new();
        public List<DynamicColumnValueDetailViewModel> DynamicColumns { get; set; } = new();



        public string? DefaultImageUrl { get; set; }
        public IFormFile? File { get; set; }
        public string? FileType { get; set; }
        public string GetBaseFolder()
        {
            var ext = FileType;
            if (ext == ".jpg" || ext == ".jpeg" || ext == ".png")
            {
                return "Images";
            }
            if (ext == ".mp4")
            {
                return "Videos";
            }
            if (ext == ".pdf" || ext == ".docx" || ext == ".xlsx" || ext == ".txt")
            {
                return "Documents";
            }
            return "Others";
        }
    }
}
