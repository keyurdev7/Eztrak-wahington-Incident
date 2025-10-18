using Models.Common.Interfaces;
using System.ComponentModel.DataAnnotations;
using ViewModels.Shared;

namespace ViewModels
{
    public class TransactionModifyViewModel : BaseUpdateVM, IBaseCrudViewModel, IIdentitifier
    {
        [Display(Name = "PO No", Prompt = "PO No")]
        public string? LotNO { get; set; }

        [Display(Name = "Quantity", Prompt = "Quantity")]
        [Required]
        public double Quantity { get; set; }

        [Display(Name = "Rate", Prompt = "Rate")]
        [Required]
        public double ItemPrice { get; set; }


        public SourceBriefViewModel Source { get; set; } = new();
        public SupplierBriefViewModel Supplier { get; set; } = new();
        public LocationBriefViewModel Location { get; set; } = new();
        public long InventoryId { get; set; }
    }

}

