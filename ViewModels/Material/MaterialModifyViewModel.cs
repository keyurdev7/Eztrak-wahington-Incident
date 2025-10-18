using System.ComponentModel.DataAnnotations;
using Models.Common.Interfaces;
using ViewModels.Shared;

namespace ViewModels
{
    public class MaterialModifyViewModel : BaseUpdateVM, IBaseCrudViewModel, IIdentitifier
    {
        [Display(Name = "Material ID", Prompt = "e.g., MAT-001")]
        [Required]
        public string MaterialID { get; set; }

        [Display(Name = "Name", Prompt = "Material name")]
        [Required]
        public string Name { get; set; }

        [Display(Name = "Category", Prompt = "Select Category")]
        public string? Category { get; set; }

        [Display(Name = "Unit", Prompt = "Select Unit")]
        public string? Unit { get; set; }

        [Display(Name = "Unit Cost", Prompt = "Enter unit cost")]
        [Range(0, long.MaxValue, ErrorMessage = "Unit cost must be a positive number.")]
        public float UnitCost { get; set; }
    }
}
