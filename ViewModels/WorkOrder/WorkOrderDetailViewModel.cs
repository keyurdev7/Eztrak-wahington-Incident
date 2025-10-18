using Enums;
using Helpers.Datetime;
using Microsoft.AspNetCore.Http;
using Models.Common.Interfaces;
using System.ComponentModel.DataAnnotations;
using ViewModels.Manager;
using ViewModels.Shared;
using ViewModels.Shared.Notes;
using ViewModels.WorkOrder;
using ViewModels.WorkOrder.CostPerformance;
using ViewModels.WorkOrderTechnician;

namespace ViewModels
{
    public class WorkOrderDetailViewModel : BaseCrudViewModel, ISelect2BaseVM, IHasNotes, INullableIdentitifier, IDynamicColumns
    {
        public long? Id { get; set; }
        public string Select2Text { get => SystemGeneratedId?.ToString(); }
        public bool HasNotes { get; set; }

        public string HasNotesClass
        {
            get
            {
                return HasNotes ? "has-note" : "";
            }
        }
        [Display(Name = "Work Order #")]
        public string? SystemGeneratedId { get; set; }

        [Display(Name = "Task", Prompt = "Task")]
        public TaskCatalog Task { get; set; }

        [Display(Name = "Type", Prompt = "Type")]
        public WorkOrderTypeCatalog Type { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public WOStatusCatalog Status { get; set; }
        public Urgency Urgency { get; set; }
        public string FormattedUrgency { get { return Urgency.ToString(); } }
        [Display(Name = "Total Hours")]
        public double TotalHours { get; set; }
        public double LabourCost { get; set; }
        public double EquipmentCost { get; set; }
        public double MaterialCost { get; set; }
        [Display(Name = "Total Cost")]
        public double TotalCost { get; set; }
        public double ActualHours { get; set; }
        public double ActualCost { get; set; }
        public ManagerBriefViewModel Manager { get; set; } = new();
        public RepairBriefViewModel Repair { get; set; } = new();
        public ReplaceBriefViewModel Replace { get; set; } = new();
        public TaskTypeBriefViewModel TaskType { get; set; } = new();
        public CostPerformanceViewModel CostPerformance { get; set; } = new();
        [Display(Name = "Date Created")]
        public DateTime? CreatedOn { get; set; }
        public string FormattedCreatedOn
        {
            get
            {
                return CreatedOn?.FormatDateInPST() ?? "-";
            }
        }

        [Display(Name = "Last Updated")]
        public DateTime? UpdatedOn { get; set; }
        public string FormattedUpdatedOn
        {
            get
            {
                return UpdatedOn?.FormatDateInPST() ?? "-";
            }
        }
        [Display(Name = "Needed By")]
        public DateTime? DueDate { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public string FormattedDueDate
        {
            get
            {
                return (DueDate != null && DueDate != DateTime.MinValue) ? DueDate.Value.FormatDate() : "-";
            }
        }
        public string FormattedApprovalDate
        {
            get
            {
                return ApprovalDate?.FormatDate() ?? "-";
            }
        }

        public long? StreetServiceRequestId { get; set; }

        public List<WorkOrderCommentViewModel> Comments { get; set; } = new();
        public List<AttachmentVM> ImagesList { get; set; } = new();
        public List<IFormFile> Images { get; set; } = new();

        public List<string> ImageUrls
        {
            get
            {
                return ImagesList?.Count > 0 ? ImagesList?.Select(x => x.Url).ToList() : new List<string>();
            }
        }
        public bool CantUpdateStatus
        {
            get
            {
                return LoggedInUserRole == RolesCatalog.Technician ? (Status == WOStatusCatalog.Complete || Status == WOStatusCatalog.Approved) : (Status == WOStatusCatalog.Approved);
            }
        }
        public AssetTypeBriefViewModel? AssetType { get; set; } = new(false, "");

        [Display(Name = "Street")]

        public string? Intersection { get; set; }
        public string? AssetTypeName { get; set; }
        public WOAssetViewModel Asset { get; set; }
        public List<WorkOrderTechnicianModifyViewModel> WorkOrderTechnicians { get; set; } = new();
        public List<DynamicColumnValueDetailViewModel> DynamicColumns { get; set; } = new();
        public string? DefaultImageUrl { get; set; }
        public string FormattedDefaultImageUrl
        {
            get
            {
                return string.IsNullOrEmpty(DefaultImageUrl) ? "" : DefaultImageUrl;
            }
        }
    }
}
