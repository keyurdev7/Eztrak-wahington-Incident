using Enums;
using Helpers.Datetime;
using Helpers.Extensions;

namespace ViewModels
{
    public class TransactionReportViewModel
    {
        public long Id { get; set; }
        public string ItemNo { get; set; }
        public string? PONo { get; set; }
        public double Quantity { get; set; }
        public double ItemPrice { get; set; }
        public string CreatedBy { get; set; }
        public string TransactionType { get; set; }
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
        public string Type { get; set; }
        public string Supplier { get; set; }
        public string Location { get; set; }

        public DateTime CreatedOn { get; set; }

        public string FormattedCreatedOn
        {
            get
            {
                return CreatedOn.FormatDate();
            }
        }

        public string WorkOrderNumber { get; set; }
        public string FundingSource { get; set; }
        public string Manufacturer { get; set; }
        public string MUTCD { get; set; }
        public string UOM { get; set; }
        public double Rate { get; set; }
        public string Source { get; set; }
        public string Material { get; set; }
        public string Equipment { get; set; }
        //For WHSE Transactions we need more data, like Work Order, Funding Source, Item, Manufacturer, MUTCD, UOM, PO No, Rate, Quantity, Location, Supplier, Source, material details, equipment details
    }
}

