using System.ComponentModel.DataAnnotations;

namespace ViewModels.Authentication
{
    public class ChangePasswordVM
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current Password is Required")]
        public string CurrentPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "New Password is Required")]
        public string NewPassword { get; set; }


        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password is Required")]
        [Compare("NewPassword", ErrorMessage = "Confirm Password Does not Match with password")]
        public string ConfirmNewPassword { get; set; }


    }
}
