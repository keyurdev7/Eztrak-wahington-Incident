using Select2.Model;
using System.ComponentModel;

namespace ViewModels
{
    public class StatusLegendBriefViewModel : BaseSelect2VM, ISelect2BaseVM
    {
        public StatusLegendBriefViewModel() : base(true, "The Status legend field is required.")
        {
        }

        public StatusLegendBriefViewModel(bool isValidationEnabled, string errorMessage) : base(isValidationEnabled, errorMessage)
        {
        }

        [DisplayName("Status Legend")]
        public string? Name { get; set; }

        [DisplayName("Description")]
        public string? Description { get; set; }   // <- added

        [DisplayName("Color")]
        public string? Color { get; set; }

        public override string? Select2Text
        {
            get
            {
                return $"{Name} - {Description} ({Color})";
            }
        }
    }
}
