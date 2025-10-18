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
    public class AssignResponseTeams : BaseDBModel
    {
        public string Name { get; set; }
        public string Category { get; set; }

        [RegularExpression(@"^\d{3}-[A-Z]{4}-\d{3}$",
            ErrorMessage = "Contact number must be in the format 555-DIST-001")]
        public string ContactNumber { get; set; }   // e.g. 555-DIST-001

        // Stored in DB as "Gas Leaks, Valve Repairs, Meter Issues"
        public string Specializations { get; set; }

        [NotMapped]
        public List<string> SpecializationsList
        {
            get => Specializations?.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                   .Select(s => s.Trim())
                                   .ToList() ?? new List<string>();
            set => Specializations = string.Join(", ", value);
        }
    }
}
