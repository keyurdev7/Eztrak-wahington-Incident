using Select2.Model;
using System.ComponentModel;

namespace ViewModels
{
    public class MaterialBriefViewModel : BaseSelect2VM, ISelect2BaseVM
    {
        public MaterialBriefViewModel() : base(true, "The Material field is required.")
        {
        }

        public MaterialBriefViewModel(bool isValidationEnabled, string errorMessage)
            : base(isValidationEnabled, errorMessage)
        {
        }

        [DisplayName("Material")]
        public string? Name { get; set; }

        [DisplayName("Material ID")]
        public string? MaterialID { get; set; }

        [DisplayName("Category")]
        public string? Category { get; set; }

        [DisplayName("Unit")]
        public string? Unit { get; set; }

        [DisplayName("Unit Cost")]
        public long UnitCost { get; set; }

        public override string? Select2Text => $"{Name} ({MaterialID})";
    }
}
