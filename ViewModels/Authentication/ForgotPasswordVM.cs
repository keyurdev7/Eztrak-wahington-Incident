using System.ComponentModel.DataAnnotations;

namespace ViewModels.Authentication
{
    public class ForgotPasswordVM
    {
        [Required]
        [Display(Name = "Email is Required")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}
