using Centangle.Common.ResponseHelpers.Models;
using Models.Common.Interfaces;
using Repositories.Interfaces;
using ViewModels.Authentication;
using ViewModels.Role;
using ViewModels.Shared;
using ViewModels.User.interfaces;

namespace Repositories.Common
{
    public interface IUserBaseService<CreateViewModel, UpdateViewModel, DetailViewModel> :
        IBaseCrud<CreateViewModel, UpdateViewModel, DetailViewModel>
        where DetailViewModel : class, IBaseCrudViewModel, new()
        where CreateViewModel : class, IUserCreateViewModel, IBaseCrudViewModel, new()
        where UpdateViewModel : class, IUserUpdateViewModel, IBaseCrudViewModel, IIdentitifier, new()
    {
        Task<IRepositoryResponse> Create(CreateViewModel model);
        Task<IRepositoryResponse> GetById(long id);
        Task<List<RoleBriefViewModel>> GetUserRolesForUpdation();
        Task<bool> IsEmailUnique(long id, string email);
        Task<bool> IsPinCodeUnique(long id, string PinCode);
        Task<IRepositoryResponse> ResetPinCode(ChangePinCodeVM model);
        Task<IRepositoryResponse> Update(UpdateViewModel model);
        Task<IRepositoryResponse> UpdateIdentityUser(UpdateViewModel model);
    }
}