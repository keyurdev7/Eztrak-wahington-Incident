namespace ViewModels
{
    public class ReturnInventoryItemsViewModel
    {
        public List<ReturnInventoryItemsListViewModel> Transactions { get; set; } = new();

    }
    public class ReturnInventoryItemsListViewModel : TransactionIssueViewModel
    {
        public double ReturnedQuantity { get; set; }

    }
}
