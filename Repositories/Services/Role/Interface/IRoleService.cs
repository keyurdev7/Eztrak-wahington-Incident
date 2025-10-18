using System;
using Centangle.Common.ResponseHelpers.Models;
using Models.Common.Interfaces;
using Pagination;
using Repositories.Interfaces;
using ViewModels.Shared;

namespace Repositories.Common.Role.Interface
{
    public interface IRoleService<CreateViewModel, UpdateViewModel, DetailViewModel> : IBaseCrud<CreateViewModel, UpdateViewModel, DetailViewModel>
        where DetailViewModel : class, IBaseCrudViewModel, new()
        where CreateViewModel : class, IBaseCrudViewModel, new()
        where UpdateViewModel : class, IBaseCrudViewModel, IIdentitifier, new()
    {
        Task<IRepositoryResponse> GetRoles<M>(IBaseSearchModel search);
    }
}

