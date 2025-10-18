using Models.Common.Interfaces;
using Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModels;
using ViewModels.Shared;

namespace Repositories.Common
{
    public interface IAssetTypeService<CreateViewModel, UpdateViewModel, DetailViewModel> : IBaseCrud<CreateViewModel, UpdateViewModel, DetailViewModel>
        where DetailViewModel : class, IBaseCrudViewModel, new()
        where CreateViewModel : class, IBaseCrudViewModel, new()
        where UpdateViewModel : class, IBaseCrudViewModel, IIdentitifier, new()
    {
        //Task<List<AssetTypeTreeViewModel>> GetHirearchy();

        Task<List<AssetTypeModifyViewModel>> GetAllAssetTypes();
        Task<long> SaveAssetType(AssetTypeModifyViewModel viewModel);
        Task<long> UpdateAssetType(AssetTypeModifyViewModel viewModel);
        Task<AssetTypeModifyViewModel> GetAssetTypeById(long id);
        Task<long> DeleteAssetType(long id);
    }
}
