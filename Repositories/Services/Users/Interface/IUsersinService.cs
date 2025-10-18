using Models.Common.Interfaces;
using Repositories.Interfaces;
using ViewModels;
using ViewModels.Shared;

namespace Repositories.Common
{
    public interface IUsersinService<CreateViewModel, UpdateViewModel, DetailViewModel>
    : IBaseCrud<CreateViewModel, UpdateViewModel, DetailViewModel>
    where DetailViewModel : class, IBaseCrudViewModel, new()
    where CreateViewModel : class, IBaseCrudViewModel, new()
    where UpdateViewModel : class, IBaseCrudViewModel, IIdentitifier, new()
    {
        Task<List<UserModifyViewModel>> GetAllUsers();
        Task<long> SaveUser(UserModifyViewModel viewModel);
        Task<long> UpdateUser(UserModifyViewModel viewModel);
        Task<UserModifyViewModel> GetUserById(long id);
        Task<long> DeleteUser(long id);
        Task<List<UserModifyViewModel.TeamViewModel>> GetAllTeams();
        Task<List<UserModifyViewModel.CompanyViewModel>> GetAllCompanies();
        Task<List<UserModifyViewModel.IncidentRoleViewModel>> GetAllIncidentRoles();
    }
}