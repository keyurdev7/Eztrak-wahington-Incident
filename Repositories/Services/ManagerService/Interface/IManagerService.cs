using Centangle.Common.ResponseHelpers.Models;
using Models.Common.Interfaces;
using Pagination;
using Repositories.Common.Users.Interface;
using Repositories.Interfaces;
using ViewModels.Shared;
using ViewModels.User.interfaces;

namespace Repositories.Common.ManagerService.Interface
{
    public interface IManagerService<CreateViewModel, UpdateViewModel, DetailViewModel> : IUserBaseService<CreateViewModel, UpdateViewModel, DetailViewModel>
        where DetailViewModel : class, IBaseCrudViewModel, new()
        where CreateViewModel : class, IUserCreateViewModel, IBaseCrudViewModel, new()
        where UpdateViewModel : class, IUserUpdateViewModel, IBaseCrudViewModel, IIdentitifier, new()
    {
        Task<IRepositoryResponse> GetDataForSelect2<M>(IBaseSearchModel model);
    }
}

