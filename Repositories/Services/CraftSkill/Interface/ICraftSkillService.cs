using System;
using Centangle.Common.ResponseHelpers.Models;
using Models.Common.Interfaces;
using Pagination;
using Repositories.Interfaces;
using ViewModels.Shared;

namespace Repositories.Common
{
    public interface ICraftSkillService<CreateViewModel, UpdateViewModel, DetailViewModel> : IBaseCrud<CreateViewModel, UpdateViewModel, DetailViewModel>
        where DetailViewModel : class, IBaseCrudViewModel, new()
        where CreateViewModel : class, IBaseCrudViewModel, new()
        where UpdateViewModel : class, IBaseCrudViewModel, IIdentitifier, new()
    {
        Task<IRepositoryResponse> GetCraftSkillsForSelect2<M>(IBaseSearchModel search);
    }
}

