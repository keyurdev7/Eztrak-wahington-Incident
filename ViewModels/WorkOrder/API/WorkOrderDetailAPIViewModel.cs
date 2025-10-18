using Enums;
using Helpers.Extensions;
using System.ComponentModel.DataAnnotations;
using ViewModels.Shared;

namespace ViewModels
{
    public class WorkOrderDetailAPIViewModel
    {
        public long? Id { get; set; }
        public string? SystemGeneratedId { get; set; }
        public WOStatusCatalog Status { get; set; }
        public Urgency Urgency { get; set; }
        public string FormattedUrgency { get { return Urgency.GetEnumDescription(); } }
        [Display(Name = "Task", Prompt = "Task")]
        public TaskCatalog Task { get; set; }
        public BaseMinimalVM TaskType { get; set; } = new();
        [Display(Name = "Type", Prompt = "Type")]
        public WorkOrderTypeCatalog Type { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Manager { get; set; }
        public string? Location { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime ApprovalDate { get; set; }
        public AssetWorkOrderAPIViewModel Asset { get; set; } = new();
    }

}
