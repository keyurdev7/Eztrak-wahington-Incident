using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using ViewModels.Shared;
using Enums;

namespace ViewModels.Authentication
{
    public class ChangePinCodeVM 
    {
        [DisplayName("New Pin Code")]
        [RegularExpression(@"^(\d{4})$", ErrorMessage = "Pin Code must be of 4-digits.")]
        [Remote(action: "ValidatePinCode", controller: "User", AdditionalFields = "Id,PinCode", ErrorMessage = "Pin Code already in use.")]
        public string PinCode { get; set; }
        public long Id { get; set; }
        public string? CurrentPinCode { get; set; }


        [Required]
        [DisplayName("Confirm Pin Code")]
        [Compare("PinCode", ErrorMessage = "Confirm Pin Code Does not Match.")]
        public string ConfirmPinCode { get; set; }

    }
}
