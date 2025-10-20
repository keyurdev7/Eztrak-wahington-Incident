using Models.Models.Shared;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class IncidentValidationTask : BaseDBModel
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
        //public DateTime? StartTime { get; set; }
        //public DateTime? ComplateTime { get; set; }
    }
}
