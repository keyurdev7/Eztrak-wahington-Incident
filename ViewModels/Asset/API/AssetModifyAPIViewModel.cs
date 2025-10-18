using Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Common.Interfaces;
using System.ComponentModel.DataAnnotations;
using ViewModels.Shared;

namespace ViewModels
{
    public class AssetModifyAPIViewModel
    {
        public long Id { get; set; }
        [Display(Name = "Asset Id", Prompt = "Asset Id")]
        [Remote(action: "IsAssetIdUnique", controller: "Asset", AdditionalFields = "Id,AssetId", ErrorMessage = "Asset Id already in use.")]
        public long AssetId { get; set; }
        [Display(Name = "Pole Id", Prompt = "Pole Id")]

        public string PoleId { get; set; }
        [Display(Prompt = "Description")]
        public string Description { get; set; }

        [Display(Name = "Class", Prompt = "Class")]

        public string AssetClass { get; set; }
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
        [Display(Name = "Next Maintenance Date (in years)", Prompt = "Next Maintenance Date")]
        public MaintenanceCycleCatalog? NextMaintenanceYear { get; set; }

        public long AssetTypeId { get; set; }
        public long ManufacturerId { get; set; }
        public long ConditionId { get; set; }
        public long MUTCDId { get; set; }
        public List<IFormFile> Images { get; set; } = new();

        public List<AssetAssociationAPIViewModel> AssetAssociation { get; set; } = new();
    }
}
