// ViewModels/IncidentRoleBriefViewModel.cs
using Select2.Model;
using System.ComponentModel;

namespace ViewModels
{
    public class IncidentRoleBriefViewModel : BaseSelect2VM, ISelect2BaseVM
    {
        public IncidentRoleBriefViewModel()
            : base(true, "The Incident Role field is required.")
        {
        }

        public IncidentRoleBriefViewModel(bool isValidationEnabled, string errorMessage)
            : base(isValidationEnabled, errorMessage)
        {
        }

        [DisplayName("Incident Role")]
        public string? Name { get; set; }

        [DisplayName("Description")]
        public string? Description { get; set; }

        public override string? Select2Text
        {
            get
            {
                return $"{Name} - {Description}";
            }
        }
    }
}
