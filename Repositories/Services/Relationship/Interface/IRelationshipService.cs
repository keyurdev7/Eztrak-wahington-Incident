using Models.Common.Interfaces;

using Repositories.Interfaces;

using System;

using ViewModels;
using ViewModels.Shared;

namespace Repositories.Common
{
    public interface IRelationshipService<CreateViewModel, UpdateViewModel, DetailViewModel>
        : IBaseCrud<CreateViewModel, UpdateViewModel, DetailViewModel>
        where DetailViewModel : class, IBaseCrudViewModel, new()
        where CreateViewModel : class, IBaseCrudViewModel, new()
        where UpdateViewModel : class, IBaseCrudViewModel, IIdentitifier, new()
    {
        Task<List<RelationshipModifyViewModel>> GetAllRelationships();
        Task<long> SaveRelation(RelationshipModifyViewModel viewModel);
        Task<long> UpdateRelation(RelationshipModifyViewModel viewModel);
        Task<RelationshipModifyViewModel> GetRelationById(long Id);
        Task<long> DeleteRelation(long id);
    }
}
    