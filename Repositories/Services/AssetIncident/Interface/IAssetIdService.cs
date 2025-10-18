using Models.Common.Interfaces;

using Repositories.Interfaces;

using System;

using ViewModels;
using ViewModels.Shared;

namespace Repositories.Common
{
    public interface IAssetIdService<CreateViewModel, UpdateViewModel, DetailViewModel>
        : IBaseCrud<CreateViewModel, UpdateViewModel, DetailViewModel>
        where DetailViewModel : class, IBaseCrudViewModel, new()
        where CreateViewModel : class, IBaseCrudViewModel, new()
        where UpdateViewModel : class, IBaseCrudViewModel, IIdentitifier, new()
    {
        Task<List<AssetIdModifyViewModel>> GetAllAssetIds();
        Task<long> SaveAssetId(AssetIdModifyViewModel viewModel);
        Task<long> UpdateAssetId(AssetIdModifyViewModel viewModel);
        Task<AssetIdModifyViewModel> GetAssetIdById(long id);
        Task<long> DeleteAssetId(long id);
    }
}
