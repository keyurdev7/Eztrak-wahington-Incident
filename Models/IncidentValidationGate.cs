using Models.Models.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class IncidentValidationGate : BaseDBModel
    {
        [Required]
        [ForeignKey("Incident")]
        public long IncidentId { get; set; }

        [Required]
        [ForeignKey("IncidentValidation")]
        public long IncidentValidationId { get; set; }

        public bool? ContainmentAcknowledgement { get; set; }
        public bool? Exception { get; set; }
        public bool? IndependentInspection { get; set; }
        public string? Regulatory { get; set; }
        public bool IsOtherEvent { get; set; }
        public string? OtherEventDetail { get; set; }
    }
}
