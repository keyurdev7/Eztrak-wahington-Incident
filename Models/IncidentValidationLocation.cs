using Models.Models.Shared;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class IncidentValidationLocation : BaseDBModel
    {
        [ForeignKey("IncidentId")]
        public long? IncidentId { get; set; }
        public Incident Incident { get; set; }

        [ForeignKey("IncidentValidationId")]
        public long? IncidentValidationId { get; set; }
        public IncidentValidation IncidentValidation { get; set; }

        [ForeignKey("AdditionalLocationId")]
        public long? AdditionalLocationId { get; set; }
        public AdditionalLocations AdditionalLocation { get; set; }

        public long? ConfirmedSeverityLevelId { get; set; }
        public long? DiscoveryPerimeterId { get; set; }
        public string? ICPLocation { get; set; }
        public string? Source { get; set; }
        public double? Lat { get; set; }
        public double? Lng { get; set; }
    }
}
