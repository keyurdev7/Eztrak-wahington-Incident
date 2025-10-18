using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Models.Common.Interfaces;
using ViewModels.Shared;
using Microsoft.AspNetCore.Http;
using ViewModels.User.interfaces;
using ViewModels.Role;

namespace ViewModels.Users
{
    public class UserUpdateViewModel : UserCreateViewModel, IBaseCrudViewModel, IUserUpdateViewModel, IIdentitifier
    {
        public UserUpdateViewModel()
        {

        }

        public List<RoleBriefViewModel> Roles { get; set; } = new List<RoleBriefViewModel>();
    }
}
