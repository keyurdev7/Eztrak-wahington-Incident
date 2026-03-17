using Models.Models.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class AdditionalLocations : BaseDBModel
    {
        [ForeignKey("IncidentID")]
        public Incident Incident { get; set; }
        public long? IncidentID { get; set; }
        public string? LocationAddress { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? NearestIntersection { get; set; }
        public string? ServiceAccount { get; set; }
        public bool PerimeterType { get; set; }
        public long? PerimeterTypeDigit { get; set; }
        public string? AssetIds { get; set; }
        public bool IsPrimaryLocation { get; set; }

        // Verification/import support (per-incident)
        public bool IsVerificationPoint { get; set; }
        public string? VerificationStatus { get; set; } // Pending / Verified / Rejected
        public string? VerificationNotes { get; set; }
        public string? VerificationPhotoUrl { get; set; } // '|' separated URLs
        public DateTime? VerifiedOn { get; set; }
        public long? VerifiedByUserId { get; set; }
        public string? VerifiedByUserName { get; set; }
        public Guid? ImportBatchId { get; set; }
    }
}