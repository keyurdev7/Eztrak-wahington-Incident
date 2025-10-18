using System;
namespace ViewModels
{
    public class IssueEquipmentItemViewModel
    {
        public IssueEquipmentItemViewModel()
        {
        }
        public EquipmentDetailViewModel Equipment { get; set; }
        public OrderItemDetailViewModel OrderItem { get; set; }
        public List<IssueEquipmentItemListViewModel> Transactions { get; set; }
    }

    public class IssueEquipmentItemListViewModel : EquipmentTransactionIssueViewModel
    {
        public LocationBriefViewModel NewLocation { get; set; } = new();
        public double NewQuantity { get; set; }
    }
}

