using System.ComponentModel.DataAnnotations;
using ViewModels.Shared;
using ViewModels.Users;

namespace ViewModels.Employee
{
    public class EmployeeDetailViewModel : BaseCrudViewModel, IEmailVM
    {
        public long Id { get; set; }
        [Display(Name = "First Name")]

        public string? FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string? LastName { get; set; }
        public string Email { get; set; }
        //public long UserId { get; set; }

        //public string? AddressLine1 { get; set; }

        //public string? AddressLine2 { get; set; }

        //public string? City { get; set; }

        //public string? State { get; set; }

        //public string? Zip { get; set; }

    }
}
