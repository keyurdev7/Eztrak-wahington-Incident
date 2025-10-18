using Pagination;

namespace ViewModels
{
    public class OrderItemSearchViewModel : BaseSearchModel
    {
        public InventoryBriefViewModel Inventory { get; set; } = new();
        public OrderDetailViewModel Order { get; set; } = new();
    }
}
