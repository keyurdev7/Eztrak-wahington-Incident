using Select2.Model;
using System.ComponentModel;

namespace ViewModels
{
    public class CategoryBriefViewModel : BaseSelect2VM, ISelect2BaseVM
    {
        public CategoryBriefViewModel() : base(true, "The Category field is required.")
        {

        }
        public CategoryBriefViewModel(bool isValidationEnabled, string errorMessage) : base(isValidationEnabled, errorMessage)
        {

        }
        [DisplayName("Category")]
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
