using ViewModels;

namespace Repositories.Common
{
    public interface IExecuteEquipmentService
    {

        Task<List<EquipmentTransactionIssueViewModel>> GetGroupedOrderTransactionsByEquipments(string poNo);
        Task<bool> ReturnEquipments(ReturnEquipmentViewModel viewModel);
    }
}
