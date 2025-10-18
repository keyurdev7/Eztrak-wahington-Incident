using System.ComponentModel;

namespace ViewModels
{
    public class AssetTypeLevel2BriefViewModel : BaseSelect2VM, ISelect2BaseVM
    {
        public AssetTypeLevel2BriefViewModel() : base(true, "The AssetTypeLevel2 field is required.")
        {

        }
        [DisplayName("AssetTypeLevel2")]
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
