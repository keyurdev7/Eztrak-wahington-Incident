using Enums;
using Helpers.Datetime;
using Models;
using ViewModels.Shared;

namespace ViewModels
{
    public class EquipmentTransactionDetailViewModel : BaseCrudViewModel
    {
        public long Id { get; set; }
        public string ItemNo { get; set; }
        public string? PONo { get; set; }
        public double Quantity { get; set; }
        public double ItemPrice { get; set; }
        public double HourlyRate { get; set; }
        public double Hours { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string FormattedPurchaseDate
        {
            get
            {
                return PurchaseDate.FormatDate();
            }
        }
        public string FormattedItemPrice
        {
            get
            {
                return ItemPrice.ToString("C");
            }
        }
        public SupplierBriefViewModel Supplier { get; set; } = new();
        public LocationBriefViewModel Location { get; set; } = new();
        public EquipmentDetailViewModel Equipment { get; set; } = new();
        public ConditionBriefViewModel Condition { get; set; } = new();
        public EquipmentTransactionTypeCatalog TransactionType { get; set; }
        public DateTime CreatedOn { get; set; }

        public long? EntityId { get; set; }
        public string EntityName { get; set; }
        public long? EntityDetailId { get; set; }
        public string EntityDetailName { get; set; }

    }
}
