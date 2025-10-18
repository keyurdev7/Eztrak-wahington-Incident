using Models.Common.Interfaces;
using Models.Models.Shared;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class IncidentUser : BaseDBModel
    {
        public long TeamId { get; set; }
        [ForeignKey(nameof(TeamId))]
        public IncidentTeam Team { get; set; }

        public long? CompanyId { get; set; }
        [ForeignKey(nameof(CompanyId))]
        public Company Company { get; set; }

        public long? IncidentRoleId { get; set; }
        [ForeignKey(nameof(IncidentRoleId))]
        public IncidentRole IncidentRole { get; set; }
        
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }

        // NOTE: store hashed PIN in production. Property named PinHash to avoid storing plaintext.
        public string PinHash { get; set; }

        [NotMapped]
        public string Pin { get; set; } // for incoming form only (not persisted)

        [NotMapped]
        public string VerifyPin { get; set; } // for incoming form only (not persisted)
        public string? EmployeeType { get; set; } // e.g. "Employee", "Contractor"
    }
}
