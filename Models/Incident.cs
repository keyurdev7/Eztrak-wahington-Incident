using Models.Models.Shared;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Incident : BaseDBModel
    {
        public string IncidentID { get; set; }

        [ForeignKey("RelationshipId")]
        public long? RelationshipId { get; set; }
        public Relationship Relationship { get; set; }

        //[ForeignKey("EventTypeId")]
        //public long? EventTypeId { get; set; }
        //public EventType EventType { get; set; }

        public string? EventTypeIds { get; set; }

        [ForeignKey("SeverityLevelId")]
        public long? SeverityLevelId { get; set; }
        public SeverityLevel SeverityLevel { get; set; }

        [ForeignKey("StatusLegendId")]
        public long? StatusLegendId { get; set; }
        public StatusLegend StatusLegend { get; set; }

        public string? CallerName { get; set; }
        public string? CallerPhoneNumber { get; set; }
        public string? CallerAddress { get; set; }
        public DateTime CallTime { get; set; }
        public string? LocationAddress { get; set; }
        public string? Landmark { get; set; }
        public string? ServiceAccount { get; set; }
        public string? AssetIds { get; set; }
        public string? DescriptionIssue { get; set; }
        public long? GasPresentId { get; set; }
        public long? WaterPresentId { get; set; }
        public long? HissingPresentId { get; set; }
        public long? VisibleDamagePresentId { get; set; }
        public long? PeopleInjuredId { get; set; }
        public long? EvacuationRequiredId { get; set; }
        public long? EmergencyResponseNotifiedId { get; set; }
        public string? ImageUrl { get; set; }
        public string? SupportInfoNotes { get; set; }
        public bool IsSameCallerAddress { get; set; }
        public bool IsOtherEvent { get; set; }
        public string? OtherEventDetail { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        public string? Phase { get; set; }
        public string? Progress { get; set; }
    }
}
