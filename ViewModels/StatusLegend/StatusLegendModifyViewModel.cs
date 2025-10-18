using System.ComponentModel.DataAnnotations;
using Models.Common.Interfaces;
using ViewModels.Shared;
using Enums;

namespace ViewModels
{
    public class StatusLegendModifyViewModel : BaseUpdateVM, IBaseCrudViewModel, IIdentitifier
    {
        [Display(Name = "Status Legend", Prompt = "Status Legend")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [Display(Name = "Description", Prompt = "Description")]
        public string? Description { get; set; }   // <- added

        [Required(ErrorMessage = "Color is required.")]
        [Display(Name = "Color", Prompt = "Color (Hex Code or Name)")]
        public string Color { get; set; }
    }
}
