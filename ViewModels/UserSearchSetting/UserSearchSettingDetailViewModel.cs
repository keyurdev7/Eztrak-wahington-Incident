using System.ComponentModel.DataAnnotations;
using Enums;
using ViewModels.Shared;

namespace ViewModels
{
    public class UserSearchSettingDetailViewModel : BaseCrudViewModel
    {
        public long Id { get; set; }

        [Required]
        [Display(Name = "Name", Prompt = "Name")]
        public string Name { get; set; }
        public SearchFilterTypeCatalog Type { get; set; }

        public string FilterJson { get; set; }
        public string ClassName { get; set; }
        public string ViewPath { get; set; }
    }
}
