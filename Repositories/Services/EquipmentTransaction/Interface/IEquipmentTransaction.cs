using Models.Common.Interfaces;
using Repositories.Interfaces;
using ViewModels;
using ViewModels.Shared;

namespace Repositories.Common
{
    public interface IEquipmentTransactionService<CreateViewModel, UpdateViewModel, DetailViewModel> : IBaseCrud<CreateViewModel, UpdateViewModel, DetailViewModel>
      where DetailViewModel : class, IBaseCrudViewModel, new()
      where CreateViewModel : class, IBaseCrudViewModel, new()
      where UpdateViewModel : class, IBaseCrudViewModel, IIdentitifier, new()
    {
        Task<List<EquipmentTransactionDetailViewModel>> GetGroupedTransactionsByItems(List<long> inventoryId);
        Task<List<EquipmentTransactionDetailViewModel>> GetGroupedTransactionsByItemsForOrder(List<long> inventoryId);
        Task<List<EquipmentTransactionDetailViewModel>> GetWorkOrderTransactions(string WorkOrderId);
    }
}
