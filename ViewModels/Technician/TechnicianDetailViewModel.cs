using System.ComponentModel.DataAnnotations;
using ViewModels.Shared;
using ViewModels.Users;

namespace ViewModels.Technician
{
    public class TechnicianDetailViewModel : BaseCrudViewModel, IEmailVM
    {
        public long Id { get; set; }
        [Display(Name = "First Name")]

        public string? FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string? LastName { get; set; }

        public string Name { get => $"{FirstName} {LastName}"; }
        public string Email { get; set; }
        public string? PinCode { get; set; }
    }
}
