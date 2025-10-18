using Models.Models.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class IncidentValidationNotes : BaseDBModel
    {
        [ForeignKey("Incident")]
        public long? IncidentId { get; set; }
        public Incident Incident { get; set; }

        [ForeignKey("IncidentValidation")]
        public long? IncidentValidationId { get; set; }
        public IncidentValidation IncidentValidation { get; set; }

        public string? Notes { get; set; }
    }
}
