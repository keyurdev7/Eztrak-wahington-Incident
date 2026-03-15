using Models.Models.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class IncidentValidationAssessmentTask : BaseDBModel
    {
        [ForeignKey("IncidentId")]
        public long? IncidentId { get; set; }
        public Incident Incident { get; set; }

        [ForeignKey("IncidentValidationId")]
        public long? IncidentValidationId { get; set; }
        public IncidentValidation IncidentValidation { get; set; }

        public string RoleIds { get; set; }
        public long? StatusId { get; set; }
        public string TaskDescription { get; set; }
        public string? Notes { get; set; }
        public string? ImageUrls { get; set; }

        /// <summary>
        /// User-defined ordering for display in the Assessment tab.
        /// </summary>
        public int SortOrder { get; set; }
    }
}
