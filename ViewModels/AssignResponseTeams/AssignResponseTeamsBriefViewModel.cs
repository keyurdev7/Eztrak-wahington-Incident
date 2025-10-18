using System.ComponentModel;

namespace ViewModels
{
    public class AssignResponseTeamsBriefViewModel : BaseSelect2VM, ISelect2BaseVM
    {
        public AssignResponseTeamsBriefViewModel() : base(true, "The Assign Response Teams ID field is required.")
        {
        }

        public AssignResponseTeamsBriefViewModel(bool isValidationEnabled, string errorMessage)
            : base(isValidationEnabled, errorMessage)
        {
        }

        [DisplayName("Assign Response Teams ID")]
        public string? Name { get; set; }

        public override string? Select2Text
        {
            get
            {
                return Name;
            }
        }
    }
}
