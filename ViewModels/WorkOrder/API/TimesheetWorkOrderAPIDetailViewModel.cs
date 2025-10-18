using Enums;
using Helpers.Extensions;
using System.ComponentModel.DataAnnotations;
using ViewModels.Shared;

namespace ViewModels
{
    public class TimesheetWorkOrderDetailAPIViewModel
    {
        public long? Id { get; set; }
        public string? SystemGeneratedId { get; set; }
        public WOStatusCatalog Status { get; set; }
        public Urgency Urgency { get; set; }
        public BaseMinimalVM AssetType { get; set; } = new();
        public string FormattedUrgency { get { return Urgency.GetEnumDescription(); } }
        [Display(Name = "Type", Prompt = "Type")]
        public WorkOrderTypeCatalog Type { get; set; }
        [Display(Name = "Task", Prompt = "Task")]
        public TaskCatalog Task { get; set; }
        public DateTime? DueDate { get; set; }
    }

}
