using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Models.Shared;

namespace Models
{
    public class IncidentValidationAssignedRole : BaseDBModel
    {
        [Required]
        [ForeignKey("Incident")]
        public long IncidentId { get; set; }

        [Required]
        [ForeignKey("IncidentValidation")]
        public long IncidentValidationId { get; set; }

        public long? IncidentCommander { get; set; }
        public long? FieldEnvRep { get; set; }
        public long? GEC_Coordinator { get; set; }
        public long? EngineeringLead { get; set; }

    }
}
