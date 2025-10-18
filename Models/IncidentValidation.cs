using Models.Models.Shared;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class IncidentValidation : BaseDBModel
    {
        [ForeignKey("IncidentId")]
        public long? IncidentId { get; set; }
        public Incident Incident { get; set; }
        public long ConfirmedSeverityLevelId { get; set; }
        public long DiscoveryPerimeterId { get; set; }
        public string AssignResponseTeams { get; set; }
        public string ValidationNotes { get; set; }
        public bool IsMarkFalseAlarm { get; set; }
    }
}
