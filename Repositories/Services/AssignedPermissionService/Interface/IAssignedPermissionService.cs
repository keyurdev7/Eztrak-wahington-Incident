using System;
using Centangle.Common.ResponseHelpers.Models;
using Enums;
using Models.Common.Interfaces;
using Pagination;
using Repositories.Interfaces;
using ViewModels.AssignedPermissions;
using ViewModels.Role;
using ViewModels.Shared;
using ViewModels.Users;

namespace Repositories.Common.Example.Interface
{
    public interface IAssignedPermissionService<CreateViewModel, UpdateViewModel, DetailViewModel> : IBaseCrud<CreateViewModel, UpdateViewModel, DetailViewModel>
        where DetailViewModel : class, IBaseCrudViewModel, new()
        where CreateViewModel : class, IBaseCrudViewModel, new()
        where UpdateViewModel : class, IBaseCrudViewModel, IIdentitifier, new()
    {
        Task<IRepositoryResponse> AssignPermissions(AssignPermissionDataViewModel model);
        Task<IRepositoryResponse> GetAssignedPermissions(IBaseSearchModel search);
        Task<UserBriefViewModel> GetUser(long entityId);
        Task<RoleBriefViewModel> GetRole(long entityId);
        Task<List<string>> SetUserPermissions(string cacheKey, string userIdentifierKey, long userId, List<long> roleIdList);
    }
}

