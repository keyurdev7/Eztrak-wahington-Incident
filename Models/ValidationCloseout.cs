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
    public class ValidationCloseout : BaseDBModel
    {
        [Required]
        [ForeignKey("Incident")]
        public long IncidentId { get; set; }

        [Required]
        [ForeignKey("IncidentValidation")]
        public long IncidentValidationId { get; set; }
        public string? Description { get; set; }
        public string? Role { get; set; }
        public string? Status { get; set; }
        public string? Notes { get; set; }
        public string? ImageUrls { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? ComplateTime { get; set; }
    }
}
