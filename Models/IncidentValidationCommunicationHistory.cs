using Models.Models.Shared;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class IncidentValidationCommunicationHistory: BaseDBModel
    {
        [ForeignKey("IncidentId")]
        public long? IncidentId { get; set; }
        public Incident Incident { get; set; }

        [ForeignKey("IncidentValidationId")]
        public long IncidentValidationId { get; set; }
        public IncidentValidation IncidentValidation { get; set; }
        public string UserName { get; set; }
        public string Message { get; set; }
        public string TimeStamp { get; set; }
        public string RecipientsIds { get; set; }
        public string ImageUrl { get; set; }
        public long MessageType { get; set; }
    }
}
