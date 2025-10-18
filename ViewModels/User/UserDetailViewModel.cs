using Enums;
using Select2.Model;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ViewModels.Role;
using ViewModels.Shared;

namespace ViewModels.Users
{
    public class UserDetailViewModel : BaseCrudViewModel, IEmailVM
    {
        public long Id { get; set; }
        public string Name { get => string.IsNullOrEmpty(FirstName) && string.IsNullOrEmpty(LastName) ? string.IsNullOrEmpty(Username) ? "-" : $"{Username}" : $"{FirstName} {LastName}"; set { } }

        [DisplayName("Name")]
        public string FirstName { get; set; }
        [DisplayName("Last Name")]
        public string LastName { get; set; }
        [DisplayName("Image")]
        public string ImageUrl { get; set; }
        public string? Email { get; set; }
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        public string Image { get => string.IsNullOrEmpty(ImageUrl) ? "/Storage/Default/default.jpg" : ImageUrl; }

        public string Username { get; set; }

        public RoleBriefViewModel Role { get; set; }

        public bool CantResetPinCode
        {
            get
            {
                if (Role?.Name != RolesCatalog.Technician.ToString())
                {
                    return true;
                }
                return false;
            }
            
        }
        public bool CantResetPassword
        {
            get
            {
                if (Role?.Name == RolesCatalog.Technician.ToString())
                {
                    return true;
                }
                return false;
            }

        }
    }
}
