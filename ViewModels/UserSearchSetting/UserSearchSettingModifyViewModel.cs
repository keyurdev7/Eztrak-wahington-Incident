using System.ComponentModel.DataAnnotations;
using Models.Common.Interfaces;
using ViewModels.Shared;
using Enums;

namespace ViewModels
{
    public class UserSearchSettingModifyViewModel : BaseUpdateVM, IName, IBaseCrudViewModel, IIdentitifier
    {
        [Required]
        [Display(Name = "Name", Prompt = "Name")]
        public string Name { get; set; }
        public SearchFilterTypeCatalog Type { get; set; }

        public string FilterJson { get; set; }
        public string ClassName { get; set; }
        public string ViewPath { get; set; }
    }
}
