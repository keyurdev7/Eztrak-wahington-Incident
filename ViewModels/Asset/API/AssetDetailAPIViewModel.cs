using Enums;
using Helpers.Datetime;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ViewModels.Shared;

namespace ViewModels
{
    public class AssetDetailAPIViewModel : BaseCrudViewModel, IDynamicColumns
    {
        public long Id { get; set; }
        public DateTime CreatedOn { get; set; }
        [DisplayName("Asset ID")]
        public string SystemGeneratedId { get; set; }
        public string Description { get; set; }
        public double Value { get; set; }
        [Display(Name = "Maintenance Cost")]
        public double MaintenanceCost { get; set; }
        [Display(Name = "Street")]
        public string Intersection { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        [Display(Name = "Class", Prompt = "Class")]
        public string AssetClass { get; set; }
        [Display(Name = "Installed Date")]
        public DateTime InstalledDate { get; set; }
        public string FormattedInstalledDate
        {
            get
            {
                return InstalledDate.FormatDate();
            }
        }
        [Display(Name = "Replacement Date")]
        public ReplacementCycleCatalog? ReplacementYear { get; set; }
        public string FormattedReplacementDate
        {
            get
            {
                return ReplacementDate.FormatDate();
            }
        }
        public DateTime ReplacementDate { get; set; }
        [Display(Name = "Next Maintenance Date")]
        public MaintenanceCycleCatalog? NextMaintenanceYear { get; set; }
        public DateTime NextMaintenanceDate { get; set; }
        public string FormattedNextMaintenanceDate
        {
            get
            {
                return NextMaintenanceDate.FormatDate();
            }
        }
        [DisplayName("Asset Type")]
        public BaseMinimalVM AssetType { get; set; } = new();
        public BaseMinimalVM Manufacturer { get; set; } = new();
        public BaseMinimalVM Condition { get; set; } = new();
        public BaseMinimalVM MUTCD { get; set; } = new();
        public BaseMinimalVM MountType { get; set; } = new();

        public string PoleId { get; set; }
        public List<AttachmentVM> ImagesList { get; set; } = new();

        public List<AssetAssociationDetailAPIViewModel> AssetAssociations { get; set; } = new();
        public List<DynamicColumnValueDetailViewModel> DynamicColumns { get; set; } = new();
    }
}
