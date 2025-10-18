using Enums;
using Helpers.Datetime;
using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ViewModels.Shared;

namespace ViewModels
{
    public class AssetDetailViewModel : BaseCrudViewModel, IDynamicColumns, IAssetProperties
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
        public DateTime? CreatedOn { get; set; }
        [Display(Name = "Last Updated")]
        public DateTime? UpdatedOn { get; set; }
        public string FormattedUpdatedOn
        {
            get
            {
                return UpdatedOn?.FormatDateInPST() ?? "-";
            }
        }
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
        public AssetTypeBriefViewModel AssetType { get; set; } = new();
        public ManufacturerBriefViewModel Manufacturer { get; set; } = new();
        public ConditionBriefViewModel Condition { get; set; } = new();
        public MUTCDBriefViewModel MUTCD { get; set; } = new();
        public MountTypeBriefViewModel MountType { get; set; } = new();

        public string PoleId { get; set; }


        public List<AttachmentVM> ImagesList { get; set; } = new();
        public List<IFormFile> Images { get; set; } = new();

        public List<string> ImageUrls
        {
            get
            {
                return ImagesList?.Count > 0 ? ImagesList?.Select(x => x.Url).ToList() : new List<string>();
            }
        }
        public string DataTableRowClass
        {
            get
            {
                return ReplacementDate.Date < DateTime.Now.Date ? "item-danger" : "";
            }
        }
        public List<AssetAssociationDetailViewModel> AssetAssociations { get; set; } = new();
        public List<DynamicColumnValueDetailViewModel> DynamicColumns { get; set; } = new();
    }
}
