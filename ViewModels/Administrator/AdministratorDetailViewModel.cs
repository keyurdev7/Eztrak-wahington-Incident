using System.ComponentModel.DataAnnotations;
using ViewModels.Shared;
using ViewModels.Users;

namespace ViewModels.Administrator
{
    public class AdministratorDetailViewModel : BaseCrudViewModel, IEmailVM
    {
        public long Id { get; set; }
        [Display(Name = "First Name")]

        public string? FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string? LastName { get; set; }
        public string Email { get; set; }
    }
}
