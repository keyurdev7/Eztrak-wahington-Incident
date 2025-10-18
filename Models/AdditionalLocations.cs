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
    }
}