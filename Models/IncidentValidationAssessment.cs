using Models.Models.Shared;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class IncidentValidationAssessment :BaseDBModel
    {
        [ForeignKey("IncidentId")]
        public long? IncidentId { get; set; }
        public Incident Incident { get; set; }

        [ForeignKey("IncidentValidationId")]
        public long? IncidentValidationId { get; set; }
        public IncidentValidation IncidentValidation { get; set; }

        public long? IC_MCR_AssignId { get; set; }
        public long? IC_MCR_StatusId { get; set; }

        public string? IC_MCR_Notes { get; set; }
        public string? IC_MCR_ImageUrls { get; set; }
        public DateTime? IC_MCR_StartTime { get; set; }
        public DateTime? IC_MCR_ComplateTime { get; set; }

        public long? IC_Notify_AssignId { get; set; }
        public long? IC_Notify_StatusId { get; set; }

        public string? IC_Notify_Notes { get; set; }
        public string? IC_Notify_ImageUrls { get; set; }
        public DateTime? IC_Notify_StartTime { get; set; }
        public DateTime? IC_Notify_ComplateTime { get; set; }


        public long? IC_EstablishICP_AssignId { get; set; }
        public long? IC_EstablishICP_StatusId { get; set; }
        public string? IC_EstablishICP_Notes { get; set; }
        public string? IC_EstablishICP_ImageUrls { get; set; }
        public DateTime? IC_EstablishICP_StartTime { get; set; }
        public DateTime? IC_EstablishICP_ComplateTime { get; set; }

        public long? FER_PCA_AssignId { get; set; }
        public long? FER_PCA_StatusId { get; set; }
        public string? FER_PCA_Notes { get; set; }
        public string? FER_PCA_ImageUrls { get; set; }
        public DateTime? FER_PCA_StartTime { get; set; }
        public DateTime? FER_PCA_ComplateTime { get; set; }

        public long? FER_LC_AssignId { get; set; }
        public long? FER_LC_StatusId { get; set; }
        public string? FER_LC_Notes { get; set; }
        public string? FER_LC_ImageUrls { get; set; }
        public DateTime? FER_LC_StartTime { get; set; }
        public DateTime? FER_LC_ComplateTime { get; set; }

        public long? EGEC_RSM_AssignId { get; set; }
        public long? EGEC_RSM_StatusId { get; set; }
        public string? EGEC_RSM_Notes { get; set; }
        public string? EGEC_RSM_ImageUrls { get; set; }
        public DateTime? EGEC_RSM_StartTime { get; set; }
        public DateTime? EGEC_RSM_ComplateTime { get; set; }

        public long? EGEC_MLP_AssignId { get; set; }
        public long? EGEC_MLP_StatusId { get; set; }
        public string? EGEC_MLP_Notes { get; set; }
        public string? EGEC_MLP_ImageUrls { get; set; }
        public DateTime? EGEC_MLP_StartTime { get; set; }
        public DateTime? EGEC_MLP_ComplateTime { get; set; }

        public long? EGEC_ICT_AssignId { get; set; }
        public long? EGEC_ICT_StatusId { get; set; }
        public string? EGEC_ICT_Notes { get; set; }
        public string? EGEC_ICT_ImageUrls { get; set; }
        public DateTime? EGEC_ICT_StartTime { get; set; }
        public DateTime? EGEC_ICT_ComplateTime { get; set; }
    }
}
