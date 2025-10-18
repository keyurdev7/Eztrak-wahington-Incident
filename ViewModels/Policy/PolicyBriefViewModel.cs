using Select2.Model;
using System.ComponentModel;

namespace ViewModels
{
    public class PolicyBriefViewModel : BaseSelect2VM, ISelect2BaseVM
    {
        public PolicyBriefViewModel() : base(true, "The Policy field is required.")
        {
        }

        public PolicyBriefViewModel(bool isValidationEnabled, string errorMessage)
            : base(isValidationEnabled, errorMessage)
        {
        }

        [DisplayName("Policy")]
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