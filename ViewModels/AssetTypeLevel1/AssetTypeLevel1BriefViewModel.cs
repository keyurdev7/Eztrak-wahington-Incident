using System.ComponentModel;

namespace ViewModels
{
    public class AssetTypeLevel1BriefViewModel : BaseSelect2VM, ISelect2BaseVM
    {
        public AssetTypeLevel1BriefViewModel() : base(true, "The AssetTypeLevel1 field is required.")
        {

        }
        [DisplayName("Asset Type Level 1")]
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
