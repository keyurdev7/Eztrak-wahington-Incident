using System.ComponentModel.DataAnnotations;
using Models.Common.Interfaces;
using System.ComponentModel;
using ViewModels.CRUD;
using ViewModels.Shared;

namespace ViewModels
{
    public class UserManagementModifyViewModel : BaseUpdateVM, IBaseCrudViewModel, IIdentitifier
    {
        public long Id { get; set; }
        public long RoleId { get; set; }
        [DisplayName("FirstName")]
        public string FirstName { get; set; }
        [DisplayName("Email")]
        public string Email { get; set; }
        public string RoleName { get; set; }
        public string Department { get; set; }
        public string PhoneNumber { get; set; }
        public string Status { get; set; }
        /// <summary>Optional. For non-Technician roles. If empty, default password is used.</summary>
        [DataType(DataType.Password)]
        [DisplayName("Password")]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [DisplayName("Confirm Password")]
        [Compare("Password", ErrorMessage = "Confirm password does not match.")]
        public string ConfirmPassword { get; set; }
        /// <summary>Required for Technician role only. Must be 4 digits.</summary>
        [DisplayName("Pin Code")]
        [StringLength(4, MinimumLength = 4, ErrorMessage = "Pin code must be exactly 4 digits.")]
        [RegularExpression(@"^\d{4}$", ErrorMessage = "Pin code must be 4 digits.")]
        public string PinCode { get; set; }
        [DisplayName("Confirm Pin Code")]
        [Compare("PinCode", ErrorMessage = "Confirm pin code does not match.")]
        public string ConfirmPinCode { get; set; }
        public DateTime LastLogin { get; set; }
        public int totalUserCount { get; set; }
        public int activeUserCount { get; set; }
        public int inactiveUserCount { get; set; }
        public int adminsCount { get; set; }
        public class RoleViewModel
        {
            public long RoleId { get; set; }
            public string RoleName { get; set; }
        }  
        public List<RoleViewModel> Roles { get; set; } = new List<RoleViewModel>();
    }
}
