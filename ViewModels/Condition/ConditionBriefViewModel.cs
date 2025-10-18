using Select2.Model;
using System.ComponentModel;

namespace ViewModels
{
    public class ConditionBriefViewModel : BaseSelect2VM, ISelect2BaseVM
    {
        public ConditionBriefViewModel() : base(true, "The Condition field is required.")
        {

        }
        public ConditionBriefViewModel(bool isValidationEnabled,string errorMessage) : base(isValidationEnabled, errorMessage)
        {

        }
        [DisplayName("Condition")]
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
