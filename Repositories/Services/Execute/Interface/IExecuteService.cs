using ViewModels;

namespace Repositories.Common
{
    public interface IExecuteService
    {
        Task<List<TransactionIssueViewModel>> GetGroupedTransactionsByItems(string lotNo);

        Task<List<TransactionIssueViewModel>> GetGroupedOrderTransactionsByItems(string lotNo);
        Task<bool> ReStageItems(ReStageViewModel viewModel);
        Task<bool> ReturnInventoryItems(ReturnInventoryItemsViewModel viewModel);
        Task<bool> RemoveInventoryItems(RemoveInventoryItemsViewModel viewModel);
    }
}
