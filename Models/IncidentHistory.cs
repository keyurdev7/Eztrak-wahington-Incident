using Models.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class IncidentHistory : BaseDBModel
    {
        public long IncidentId { get; set; }
        public long StatusLegendId { get; set; }
        public string? Description { get; set; }
        public Incident Incident { get; set; }
        public StatusLegend StatusLegend { get; set; }
    }
}
