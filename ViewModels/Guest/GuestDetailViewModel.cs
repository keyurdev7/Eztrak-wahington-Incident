using System.ComponentModel.DataAnnotations;
using ViewModels.Shared;
using ViewModels.Users;

namespace ViewModels.Guest
{
    public class GuestDetailViewModel : BaseCrudViewModel, IEmailVM
    {
        public long Id { get; set; }
        [Display(Name = "First Name")]

        public string? FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string? LastName { get; set; }
        public string Email { get; set; }
    }
}
