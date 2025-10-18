using Select2.Model;
using System.ComponentModel;

namespace ViewModels
{
    public class ProgressBriefViewModel : BaseSelect2VM, ISelect2BaseVM
    {
        public ProgressBriefViewModel() : base(true, "The Progress field is required.")
        {

        }
        public ProgressBriefViewModel(bool isValidationEnabled, string errorMessage) : base(isValidationEnabled, errorMessage)
        {

        }
        [DisplayName("Progress")]
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
