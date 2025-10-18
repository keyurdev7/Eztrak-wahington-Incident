using Select2.Model;
using System.ComponentModel;

namespace ViewModels
{
    public class SeverityLevelBriefViewModel : BaseSelect2VM, ISelect2BaseVM
    {
        public SeverityLevelBriefViewModel()
            : base(true, "The Severity Level field is required.")
        {
        }

        public SeverityLevelBriefViewModel(bool isValidationEnabled, string errorMessage)
            : base(isValidationEnabled, errorMessage)
        {
        }

        [DisplayName("Severity Level")]
        public string? Name { get; set; }

        [DisplayName("Description")]
        public string? Description { get; set; }

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

