using Models.Common.Interfaces;
using Repositories.Interfaces;
using ViewModels;
using ViewModels.Shared;

namespace Repositories.Common
{
    public interface IUserManagementService<CreateViewModel, UpdateViewModel, DetailViewModel>
    : IBaseCrud<CreateViewModel, UpdateViewModel, DetailViewModel>
    where DetailViewModel : class, IBaseCrudViewModel, new()
    where CreateViewModel : class, IBaseCrudViewModel, new()
    where UpdateViewModel : class, IBaseCrudViewModel, IIdentitifier, new()
    {
        Task<List<UserManagementModifyViewModel.RoleViewModel>> GetAllRoles();
        Task<List<UserManagementModifyViewModel>> GetAllUserManagement();
        Task<long> SaveUserManagement(UserManagementModifyViewModel viewModel);
        Task<long> UpdateUserManagement(UserManagementModifyViewModel viewModel);
        Task<UserManagementModifyViewModel> GetUserManagementById(long id);
        Task<long> DeleteUserManagement(long id);
    }
}