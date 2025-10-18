using ViewModels.Shared;

namespace ViewModels
{
    public class TransactionDetailViewModel : BaseCrudViewModel
    {
        public long Id { get; set; }
        public string LotNO { get; set; }
        public double Quantity { get; set; }
        public double ItemPrice { get; set; }
        public string FormattedItemPrice
        {
            get
            {
                return ItemPrice.ToString("C");
            }
        }
        public SourceBriefViewModel Source { get; set; } = new();
        public SupplierBriefViewModel Supplier { get; set; } = new();
        public LocationBriefViewModel Location { get; set; } = new();
        public InventoryDetailViewModel Inventory { get; set; } = new();

        public long? EntityId { get; set; }
        public string EntityName { get; set; }
        public long? EntityDetailId { get; set; }
        public string EntityDetailName { get; set; }
        public DateTime CreatedOn { get; set; }

    }
}
