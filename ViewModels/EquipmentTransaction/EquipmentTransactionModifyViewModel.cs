using Models.Common.Interfaces;
using System.ComponentModel.DataAnnotations;
using ViewModels.Shared;

namespace ViewModels
{
    public class EquipmentTransactionModifyViewModel : BaseUpdateVM, IBaseCrudViewModel, IIdentitifier
    {

        [Display(Name = "Serial #", Prompt = "Serial #")]
        [Required]
        public string PONo { get; set; }

        [Display(Name = "Quantity", Prompt = "Quantity")]
        [Required]
        public double Quantity { get; set; }

        [Display(Name = "Rate", Prompt = "Rate")]
        [Required]
        public double ItemPrice { get; set; }

        [Display(Name = "Purchase Price", Prompt = "Purchase Price")]
        public double HourlyRate { get; set; }

        [Display(Name = "Hours", Prompt = "Hours")]
        public double Hours { get; set; }

        [Display(Name = "Purchase Date", Prompt = "Purchase Date")]
        public DateTime? PurchaseDate { get; set; }

        public SupplierBriefViewModel Supplier { get; set; } = new();
        public LocationBriefViewModel Location { get; set; } = new();
        public ConditionBriefViewModel Condition { get; set; } = new(false, "");
        public long EquipmentId { get; set; }
    }

}

