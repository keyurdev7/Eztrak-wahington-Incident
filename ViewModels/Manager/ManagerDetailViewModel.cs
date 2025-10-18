using System.ComponentModel.DataAnnotations;
using ViewModels.Shared;
using ViewModels.Users;

namespace ViewModels.Manager
{
    public class ManagerDetailViewModel : BaseCrudViewModel, IEmailVM
    {
        public long Id { get; set; }

        [Display(Name = "First Name")]
        public string? FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string? LastName { get; set; }

        public string Name { get => string.IsNullOrEmpty(FirstName) && string.IsNullOrEmpty(LastName) ? "-" : $"{FirstName} {LastName}"; }

        public string Email { get; set; }
    }
}
