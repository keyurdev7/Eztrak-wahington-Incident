using Centangle.Common.ResponseHelpers.Models;
using Models.Common.Interfaces;
using Pagination;
using Repositories.Interfaces;
using ViewModels;
using ViewModels.Shared;

namespace Repositories.Common
{
    public interface IEquipmentService<CreateViewModel, UpdateViewModel, DetailViewModel> : IBaseCrud<CreateViewModel, UpdateViewModel, DetailViewModel>
        where DetailViewModel : class, IBaseCrudViewModel, new()
        where CreateViewModel : class, IBaseCrudViewModel, new()
        where UpdateViewModel : class, IBaseCrudViewModel, IIdentitifier, new()
    {
        Task<List<EquipmentNotesViewModel>> GetNotesByEquipmentId(int id);
        Task<bool> SaveNotes(EquipmentNotesViewModel model);

        Task<IRepositoryResponse> GetTransactions(long id);
        Task<bool> CreateShipments(EquipmentShipmentGridViewModel model);

        Task<double> GetTotalEquipmentPrice(IBaseSearchModel search);
        Task<List<EquipmentCostViewModel>> GetEquipmentAverageCost(List<long> ids);
        Task<List<EquipmentTransactionHistoryViewModel>> GetEquipmentIssueHistory(int inventoryId, string poNumber, int locationId);
        Task<bool> IsItemNoUnique(long id, string itemNo);

    }
}
