using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ViewModels.Shared;

namespace ViewModels
{
    public class EquipmentDetailViewModel : BaseCrudViewModel
    {
        public long Id { get; set; }
        [DisplayName("ID #")]

        public string? SystemGeneratedId { get; set; }
        public string? FormattedSystemGeneratedId { get => string.IsNullOrEmpty(SystemGeneratedId) ? "-" : SystemGeneratedId; }
        public bool HasNotes { get; set; }

        public string HasNotesClass
        {
            get
            {
                return HasNotes ? "has-note" : "";
            }
        }
        [Display(Name = "Model")]
        public string? PONo { get; set; }
        public string? EquipmentModel { get; set; }
        public double ItemPrice { get; set; }
        public string? Description { get; set; }

        [Display(Name = "Total Value")]
        public float TotalValue { get; set; }
        [Display(Name = "Item No")]
        public string ItemNo { get; set; }

        public double Quantity { get; set; }
        public double HourlyRate { get; set; }

        public string? ImageUrl { get; set; }
        public string FormattedImageUrl
        {
            get
            {
                return string.IsNullOrEmpty(ImageUrl) ? "" : ImageUrl;
            }
        }

        public CategoryBriefViewModel Category { get; set; } = new();
        public ManufacturerBriefViewModel Manufacturer { get; set; } = new();
        public UOMBriefViewModel UOM { get; set; } = new();

        public List<EquipmentSubLineViewModel> EquipmentSublines { get; set; } = new();
    }
}
