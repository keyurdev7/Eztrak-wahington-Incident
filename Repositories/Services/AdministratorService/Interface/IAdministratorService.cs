using Models.Common.Interfaces;
using Repositories.Common.Users.Interface;
using Repositories.Interfaces;
using ViewModels.Shared;
using ViewModels.User.interfaces;

namespace Repositories.Common.AdministratorService.Interface
{
    public interface IAdministratorService<CreateViewModel, UpdateViewModel, DetailViewModel> : IUserBaseService<CreateViewModel, UpdateViewModel, DetailViewModel>
        where DetailViewModel : class, IBaseCrudViewModel, new()
        where CreateViewModel : class, IUserCreateViewModel, IBaseCrudViewModel, new()
        where UpdateViewModel : class, IUserUpdateViewModel, IBaseCrudViewModel, IIdentitifier, new()
    {
    }
}

