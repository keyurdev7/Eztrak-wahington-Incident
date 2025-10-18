using System.ComponentModel.DataAnnotations;
using Models.Common.Interfaces;
using ViewModels.Shared;
using Enums;

namespace ViewModels
{
    public class SeverityLevelModifyViewModel : BaseUpdateVM, IBaseCrudViewModel, IIdentitifier
    {
        [Display(Name = "Name", Prompt = "Name")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Description is required.")]
        [Display(Name = "Description", Prompt = "Description")]
        public string? Description { get; set; }
        [Required(ErrorMessage = "Color is required.")]
        [Display(Name = "Color", Prompt = "Color (Hex Code or Name)")]
        public string? Color { get; set; }
    }
}
