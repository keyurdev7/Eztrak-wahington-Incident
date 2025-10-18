using Select2.Model;
using System.ComponentModel;

namespace ViewModels
{
    public class AssetIdBriefViewModel : BaseSelect2VM, ISelect2BaseVM
    {
        public AssetIdBriefViewModel() : base(true, "The Asset ID field is required.")
        {
        }

        public AssetIdBriefViewModel(bool isValidationEnabled, string errorMessage)
            : base(isValidationEnabled, errorMessage)
        {
        }

        [DisplayName("Asset ID")]
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
