using System.ComponentModel;

namespace ViewModels
{
    public class AssetTypeBriefViewModel : BaseSelect2VM, ISelect2BaseVM
    {
        public AssetTypeBriefViewModel() : base(true, "The Assest Type field is required.")
        {

        }
        public AssetTypeBriefViewModel(bool isValidationEnabled, string errorMessage) : base(isValidationEnabled, errorMessage)
        {

        }
        [DisplayName("Assest Type")]
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
