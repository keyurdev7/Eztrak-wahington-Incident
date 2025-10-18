using Models.Models.Shared;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class IncidentValidationPolicy : BaseDBModel
    {
        [ForeignKey("IncidentId")]
        public long? IncidentId { get; set; }
        public Incident Incident { get; set; }

        [ForeignKey("IncidentValidationId")]
        public long IncidentValidationId { get; set; }
        public IncidentValidation IncidentValidation { get; set; }
        public long PolicyId { get; set; }
        public long Status { get; set; }
        public string TeamIds { get; set; }
    }
}
