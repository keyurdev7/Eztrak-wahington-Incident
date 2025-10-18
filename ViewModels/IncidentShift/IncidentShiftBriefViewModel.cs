using Select2.Model;
using System.ComponentModel;

namespace ViewModels
{
    public class IncidentShiftBriefViewModel : BaseSelect2VM, ISelect2BaseVM
    {
        public IncidentShiftBriefViewModel()
            : base(true, "The Incident Shift field is required.")
        {
        }

        public IncidentShiftBriefViewModel(bool isValidationEnabled, string errorMessage)
            : base(isValidationEnabled, errorMessage)
        {
        }

        [DisplayName("Incident Shift")]
        public string? Name { get; set; }

        [DisplayName("Description")]
        public string? Description { get; set; }

        public override string? Select2Text => $"{Name} - {Description}";
    }
}
