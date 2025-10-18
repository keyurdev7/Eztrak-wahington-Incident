using Enums;

namespace ViewModels
{
    public class TransactionHistoryViewModel
    {
        public long Id { get; set; }
        public string LotNO { get; set; }
        public double Quantity { get; set; }
        public double ItemPrice { get; set; }
        public TransactionTypeCatalog TransactionType { get; set; }
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
        public long InventoryId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string FormattedCreatedOn
        {
            get
            {
                return CreatedOn.ToString("MM/dd/yyyy");
            }
        }
    }
}
