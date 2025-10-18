using System.ComponentModel.DataAnnotations;
using Models.Common.Interfaces;
using ViewModels.Shared;
using Enums;
using ViewModels;

namespace ViewModels
{
    public class AssetTypeLevel1ModifyViewModel : BaseUpdateVM, IBaseCrudViewModel, IIdentitifier
    {
        [Display(Name = "Name", Prompt = "Name")]
        public string Name { get; set; } 
        public AssetTypeBriefViewModel AssetType { get; set; } = new ();
    }
}
