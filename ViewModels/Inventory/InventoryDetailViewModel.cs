using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ViewModels.Shared;

namespace ViewModels
{

    public class InventoryDetailViewModel : BaseCrudViewModel
    {
        public long Id { get; set; }

        public bool HasNotes { get; set; }

        public string HasNotesClass
        {
            get
            {
                return HasNotes ? "has-note" : "";
            }
        }
        [DisplayName("Material ID")]

        public string SystemGeneratedId { get; set; }
        public string FormattedSystemGeneratedId { get => string.IsNullOrEmpty(SystemGeneratedId) ? "-" : SystemGeneratedId; }
        public string Description { get; set; }
        public string FormattedDescription
        {
            get
            {
                return (MUTCD.Id > 0 ? MUTCD.Description : Description) ?? "";
            }
        }
        [Display(Name = "Total Value")]
        public float TotalValue { get; set; }
        [Display(Name = "ID#")]
        public double ItemPrice { get; set; }
        public string ItemNo { get; set; }
        public double Quantity { get; set; }
        [Display(Name = "Minimum Quantity")]
        public double MinimumQuantity { get; set; }
        public CategoryBriefViewModel Category { get; set; } = new();
        public ManufacturerBriefViewModel Manufacturer { get; set; } = new();
        public UOMBriefViewModel UOM { get; set; } = new();
        public MUTCDBriefViewModel MUTCD { get; set; } = new();

        public string DataTableRowClass
        {
            get
            {
                return Quantity < MinimumQuantity ? "row-low-stock" : "";
            }
        }

        public string? ImageUrl { get; set; }
        public string FormattedImageUrl
        {
            get
            {
                return string.IsNullOrEmpty(ImageUrl) ? "" : ImageUrl;
            }
        }

        public string FormattedGridImageUrl
        {
            get
            {
                return (MUTCD.Id < 1 ? ImageUrl : MUTCD.ImageUrl) ?? "";
            }
        }

    }
    public class InventoryPaginationViewModel
    {
        public List<InventoryDetailViewModel> Items { get; set; }
        public double TotalPrice { get; set; }
    }
}
