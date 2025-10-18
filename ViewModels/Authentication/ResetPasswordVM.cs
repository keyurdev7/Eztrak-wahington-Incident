using System.ComponentModel.DataAnnotations;

namespace ViewModels.Authentication
{
    public class ResetPasswordVM
    {
      
        public string Email { get; set; }

        public string? Token { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Confirm Password does not match with password")]
        public string ConfirmPassword { get; set; }

    }
}
