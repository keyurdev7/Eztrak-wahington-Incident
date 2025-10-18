using Centangle.Common.ResponseHelpers.Models;
using Enums;
using Models.Common.Interfaces;
using Repositories.Interfaces;
using ViewModels;
using ViewModels.Shared;

namespace Repositories.Common
{
    public interface IOrderService<CreateViewModel, UpdateViewModel, DetailViewModel> : IBaseCrud<CreateViewModel, UpdateViewModel, DetailViewModel>
        where DetailViewModel : class, IBaseCrudViewModel, new()
        where CreateViewModel : class, IBaseCrudViewModel, new()
        where UpdateViewModel : class, IBaseCrudViewModel, IIdentitifier, new()
    {
        Task<IRepositoryResponse> GetInventoryToIssue(long orderItemId, long inventoryId);
        Task<IRepositoryResponse> IssueInventoryItem(IssueInventoryItemViewModel model);
        Task<IRepositoryResponse> Submit(long id, OrderTypeCatalog type);
        Task<IRepositoryResponse> GetEquipmentToIssue(long orderItemId, long equipmentId);
        Task<IRepositoryResponse> IssueEquipmentItem(IssueEquipmentItemViewModel model);
    }
}
