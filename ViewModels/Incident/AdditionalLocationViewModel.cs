using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels.Incident
{
    public class AdditionalLocationViewModel1
    {
        public long Id { get; set; }
        public long? IncidentID { get; set; }
        public string LocationAddress { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string NearestIntersection { get; set; } = string.Empty;
        public string ServiceAccount { get; set; } = string.Empty;
        public bool PerimeterType { get; set; }
        public long? PerimeterTypeDigit { get; set; }
        public string AssetIds { get; set; } = string.Empty;

        // convenience property - resolved asset names
        public string AssetNames { get; set; } = string.Empty;
        public string PerimeterText { get; set; } = string.Empty;
    }
}
