using Enums;
using Models.Common.Interfaces;
using Repositories.Interfaces;
using ViewModels;
using ViewModels.Shared;

namespace Repositories.Common
{
    public interface IDynamicColumnService<CreateViewModel, UpdateViewModel, DetailViewModel> : IBaseCrud<CreateViewModel, UpdateViewModel, DetailViewModel>
        where DetailViewModel : class, IBaseCrudViewModel, new()
        where CreateViewModel : class, IBaseCrudViewModel, new()
        where UpdateViewModel : class, IBaseCrudViewModel, IIdentitifier, new()
    {
        Task<List<DynamicColumnValueDetailViewModel>> GetDynamicColumns(DynamicColumnEntityType entityType, long entityId);
        Task<bool> UpdateValues<M>(M model) where M : IDynamicColumns, IIdentitifier;
    }
}

