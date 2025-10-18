using System;
using Centangle.Common.ResponseHelpers.Models;
using Models.Common.Interfaces;
using Pagination;
using Repositories.Interfaces;
using ViewModels.Role;
using ViewModels.Shared;
using ViewModels.User.interfaces;

namespace Repositories.Common.Users.Interface
{
    public interface IUserService<CreateViewModel, UpdateViewModel, DetailViewModel> : 
        IUserBaseService<CreateViewModel, UpdateViewModel, DetailViewModel>
        where DetailViewModel : class, IBaseCrudViewModel, new()
        where CreateViewModel : class, IUserCreateViewModel, IBaseCrudViewModel, new()
        where UpdateViewModel : class, IUserUpdateViewModel, IBaseCrudViewModel, IIdentitifier, new()
    {
    }
}

