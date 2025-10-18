using System;
namespace ViewModels
{
    public class IssueInventoryItemViewModel
    {
        public IssueInventoryItemViewModel()
        {
        }
        public InventoryDetailViewModel Inventory { get; set; }
        public OrderItemDetailViewModel OrderItem { get; set; }
        public List<IssueInventoryItemListViewModel> Transactions { get; set; }
    }

    public class IssueInventoryItemListViewModel : TransactionIssueViewModel
    {
        public LocationBriefViewModel NewLocation { get; set; } = new();
        public double NewQuantity { get; set; }
    }
}

