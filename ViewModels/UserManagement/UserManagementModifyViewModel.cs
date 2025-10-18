using Models.Common.Interfaces;
using System.ComponentModel;
using System.IO.Packaging;
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
