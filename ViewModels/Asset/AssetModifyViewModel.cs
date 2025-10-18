using Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Common.Interfaces;
using System.ComponentModel.DataAnnotations;
using ViewModels.Shared;

namespace ViewModels
{
    public class AssetModifyViewModel : BaseUpdateVM, IBaseCrudViewModel, IIdentitifier, IAssetProperties, IDynamicColumns
    {
        //[Display(Name = "Asset Id", Prompt = "Asset Id")]
        //[Remote(action: "IsAssetIdUnique", controller: "Asset", AdditionalFields = "Id,AssetId", ErrorMessage = "Asset Id already in use.")]
        [Display(Name ="Asset ID")]
        public string? SystemGeneratedId { get; set; }
        [Display(Name = "Pole Id", Prompt = "Pole Id")]
        public string? PoleId { get; set; }
        [Display(Prompt = "Description")]
        public string Description { get; set; }
        [Display(Name = "Class", Prompt = "Class")]
        public string? AssetClass { get; set; }
        [Display(Prompt = "Value")]
        public double Value { get; set; }
        [Display(Name = "Maintenance Cost", Prompt = "Maintenance Cost")]
        public double MaintenanceCost { get; set; }
        [Display(Name = "Street", Prompt = "Street")]
        public string Intersection { get; set; }
        [Display(Prompt = "Latitude")]
        public double Latitude { get; set; }
        [Display(Prompt = "Longitude")]
        public double Longitude { get; set; }
        [Display(Name = "Installation Date", Prompt = "Installation Date")]
        public DateTime InstalledDate { get; set; } = DateTime.Now;
        [Display(Name = "Replacement Date (in years)", Prompt = "Replacement Date")]
        public ReplacementCycleCatalog ReplacementYear { get; set; }
        [Display(Name = "Next Replacement Date")]

        public DateTime ReplacementDate { get; set; }
        [Display(Name = "Next Maintenance Date (in years)", Prompt = "Next Maintenance Date")]
        public MaintenanceCycleCatalog? NextMaintenanceYear { get; set; }
        [Display(Name = "Next Maintenance Date")]

        public DateTime NextMaintenanceDate { get; set; }
        public AssetTypeBriefViewModel AssetType { get; set; } = new();
        public ManufacturerBriefViewModel Manufacturer { get; set; } = new();
        public ConditionBriefViewModel Condition { get; set; } = new();
        public MUTCDBriefViewModel MUTCD { get; set; } = new();
        public MountTypeBriefViewModel MountType { get; set; } = new();

        public List<AttachmentVM> ImagesList { get; set; } = new();
        public List<IFormFile> Images { get; set; } = new();
        public List<AssetAssociationDetailViewModel> AssetAssociations { get; set; } = new();
        public List<AssetTypeLevel1DetailViewModel> AssetTypeRadioButtonList { get; set; } = new();
        public List<DynamicColumnValueDetailViewModel> DynamicColumns { get; set; } = new();
    }
}
