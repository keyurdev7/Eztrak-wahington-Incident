using Select2.Model;
using System.ComponentModel;

namespace ViewModels
{
    public class CompanyBriefViewModel : BaseSelect2VM, ISelect2BaseVM
    {
        public CompanyBriefViewModel() : base(true, "The Company field is required.")
        {
        }

        public CompanyBriefViewModel(bool isValidationEnabled, string errorMessage)
            : base(isValidationEnabled, errorMessage)
        {
        }

        [DisplayName("Company")]
        public string? Name { get; set; }

        [DisplayName("Description")]
        public string? Description { get; set; }

        public override string? Select2Text
        {
            get
            {
                return !string.IsNullOrEmpty(Name) ? $"{Name} - {Description}" : Name;
            }
        }
    }
}