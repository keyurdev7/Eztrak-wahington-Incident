using Models.Models.Shared;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class IncidentTeam : BaseDBModel
    {
        public string Name { get; set; }
        public string? Department { get; set; }
        public string? Contact { get; set; }

        // CSV persisted in DB (e.g. "Gas Leaks,Valve Repairs")
        public string? Specializations { get; set; }
    }
}
