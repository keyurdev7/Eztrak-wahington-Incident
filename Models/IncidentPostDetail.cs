using Models.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class IncidentPostDetail : BaseDBModel
    {
        public long IncidentId { get; set; }
        public string? Message { get; set; }
        public string? MessageTime { get; set; }
        public long IncidentViewType { get; set; }

    }
}
