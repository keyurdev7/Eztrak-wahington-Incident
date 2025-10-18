using Select2.Model;
using System.ComponentModel;

namespace ViewModels
{
    public class IncidentTeamBriefViewModel : BaseSelect2VM, ISelect2BaseVM
    {
        public IncidentTeamBriefViewModel() : base(true, "The Incident Team field is required.")
        {
        }

        public IncidentTeamBriefViewModel(bool isValidationEnabled, string errorMessage) : base(isValidationEnabled, errorMessage)
        {
        }

        [DisplayName("Incident Team")]
        public string? Name { get; set; }

        [DisplayName("Department")]
        public string? Department { get; set; }

        [DisplayName("Contact")]
        public string? Contact { get; set; }

        [DisplayName("Specializations")]
        public string? Specializations { get; set; } // CSV form for quick display

        // Choose how Select2 will display the item. Keep concise.
        public override string? Select2Text
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Department))
                    return $"{Name} ({Department})";
                return Name;
            }
        }
    }
}
