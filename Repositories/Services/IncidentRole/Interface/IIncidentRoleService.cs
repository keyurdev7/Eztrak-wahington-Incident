// Repositories/Common/IIncidentRoleService.cs
using Models.Common.Interfaces;
using Repositories.Interfaces;
using System;
using ViewModels;
using ViewModels.Shared;

namespace Repositories.Common
{
    public interface IIncidentRoleService<CreateViewModel, UpdateViewModel, DetailViewModel>
        : IBaseCrud<CreateViewModel, UpdateViewModel, DetailViewModel>
        where DetailViewModel : class, IBaseCrudViewModel, new()
        where CreateViewModel : class, IBaseCrudViewModel, new()
        where UpdateViewModel : class, IBaseCrudViewModel, IIdentitifier, new()
    {
        Task<List<IncidentRoleModifyViewModel>> GetAllIncidentRoles();
        Task<long> SaveIncidentRole(IncidentRoleModifyViewModel viewModel);
        Task<long> UpdateIncidentRole(IncidentRoleModifyViewModel viewModel);
        Task<IncidentRoleModifyViewModel> GetIncidentRoleById(long Id);
        Task<long> DeleteIncidentRole(long id);
    }
}
