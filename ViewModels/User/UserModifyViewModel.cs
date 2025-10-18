using Models.Common.Interfaces;
using System.ComponentModel;
using ViewModels.Shared;

namespace ViewModels
{
    public class UserModifyViewModel : BaseUpdateVM, IBaseCrudViewModel, IIdentitifier
    {
        public long TeamId { get; set; }
        public long? CompanyId { get; set; }
        public long? IncidentRoleId { get; set; }
        public long Id { get; set; }
        [DisplayName("FirstName")]
        public string FirstName { get; set; }
        [DisplayName("Last Name")]
        public string LastName { get; set; }
        [DisplayName("Telephone")]
        public string Telephone { get; set; }
        [DisplayName("Email")]
        public string Email { get; set; }
        [DisplayName("PinHash")]
        public string PinHash { get; set; }
        public string VerifyPIN { get; set; }
        public string TeamName { get; set; }
        public string CompanyName { get; set; }
        public string IncidentRoleName { get; set; }
        public class TeamViewModel
        {
            public long TeamId { get; set; }
            public string TeamName { get; set; }
        }
        public List<TeamViewModel> Teams { get; set; } = new List<TeamViewModel>();
        public class CompanyViewModel
        {
            public long CompanyId { get; set; }
            public string CompanyName { get; set; }
        }
        public List<CompanyViewModel> Companies { get; set; } = new List<CompanyViewModel>();
        public class IncidentRoleViewModel
        {
            public long IncidentRoleId { get; set; }
            public string IncidentRoleName { get; set; }
        }
        public List<IncidentRoleViewModel> IncidentRoles { get; set; } = new List<IncidentRoleViewModel>();
        public string? EmployeeType { get; set; }
    }
}
