using System.ComponentModel.DataAnnotations;
using Models.Common.Interfaces;
using ViewModels.Shared;
using Enums;
using ViewModels;
using DocumentFormat.OpenXml.Wordprocessing;

namespace ViewModels
{
    public class AssetTypeLevel2ModifyViewModel : BaseUpdateVM, IBaseCrudViewModel, IIdentitifier
    {
        [Display(Name = "Name", Prompt = "Name")]
        public string Name { get; set; }
        public long AssetTypeId { get; set; }
        public AssetTypeLevel1BriefViewModel AssetTypeLevel1 { get; set; } = new ();
        
    }
}
