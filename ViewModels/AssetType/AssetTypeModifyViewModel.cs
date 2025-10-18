using Models.Common.Interfaces;
using System.ComponentModel.DataAnnotations;
using ViewModels.Shared;

namespace ViewModels
{
    public class AssetTypeModifyViewModel : BaseUpdateVM, IBaseCrudViewModel, IIdentitifier
    {
        [Display(Name = "Name", Prompt = "Name")]
        public string Name { get; set; }
        public string Color { get; set; }
    }

}
