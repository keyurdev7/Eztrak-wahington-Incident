using Centangle.Common.ResponseHelpers.Models;
using Models.Common.Interfaces;
using Repositories.Interfaces;
using ViewModels;
using ViewModels.Shared;

namespace Repositories.Common
{
    public interface IAssetService<CreateViewModel, UpdateViewModel, DetailViewModel> : IBaseCrud<CreateViewModel, UpdateViewModel, DetailViewModel>
        where DetailViewModel : class, IBaseCrudViewModel, new()
        where CreateViewModel : class, IBaseCrudViewModel, new()
        where UpdateViewModel : class, IBaseCrudViewModel, IIdentitifier, new()
    {
        Task<IRepositoryResponse> GetNotesByAssetId(long id);
        Task<IRepositoryResponse> SaveNotes(AssetNotesViewModel model);
        Task<IRepositoryResponse> RePin(AssetRePinViewModel model);
        Task<List<AttachmentVM>> GetAssetAttachments(long id);
        Task<string> GetAttachmentUrl(long id);
        Task<bool> IsAssetIdUnique(long id, string assetId);
        Task<IRepositoryResponse> GetAssetsForMap();
        Task<IRepositoryResponse> GetAssetTypeLevels(long assetTypeId, long? assetId, bool showAll = false);
        Task<IRepositoryResponse> GetAssetTypeLevelsForAPI(long assetTypeId, long assetId);
        Task<bool> InitializeExcelData(ExcelFileVM model);
        Task<IRepositoryResponse> UploadImages(AssetAddImageViewModel model);
        Task<IRepositoryResponse> UpdateAssociations(AssetModifyViewModel model);
        Task<List<AssetAssociationDetailViewModel>> GetAssetsSubLevels(AssetSearchViewModel search);
    }
}
