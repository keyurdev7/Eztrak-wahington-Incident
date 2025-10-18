using Centangle.Common.ResponseHelpers.Models;
using Models.Common.Interfaces;
using Pagination;
using Repositories.Interfaces;
using ViewModels;
using ViewModels.Shared;

namespace Repositories.Common
{
    public interface IInventoryService<CreateViewModel, UpdateViewModel, DetailViewModel> : IBaseCrud<CreateViewModel, UpdateViewModel, DetailViewModel>
        where DetailViewModel : class, IBaseCrudViewModel, new()
        where CreateViewModel : class, IBaseCrudViewModel, new()
        where UpdateViewModel : class, IBaseCrudViewModel, IIdentitifier, new()
    {
        Task<List<InventoryNotesViewModel>> GetNotesByInventoryId(int id);
        Task<bool> SaveNotes(InventoryNotesViewModel model);

        Task<IRepositoryResponse> GetTransactions(long id);
        Task<bool> CreateShipments(ShipmentGridViewModel model);

        Task<double> GetTotalInventoryPrice(IBaseSearchModel search);
        Task<List<InventoryCostViewModel>> GetInventoryAverageCost(List<long> ids);
        Task<List<TransactionHistoryViewModel>> GetInventoryIssueHistory(int inventoryId, string lotNo, int locationId, int sourceId);
        Task<bool> IsItemNoUnique(long id, string itemNo);

    }
}
