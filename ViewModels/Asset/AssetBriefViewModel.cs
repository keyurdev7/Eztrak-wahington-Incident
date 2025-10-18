using System.ComponentModel;

namespace ViewModels
{
    public class AssetBriefViewModel : BaseSelect2VM, ISelect2BaseVM
    {
        public AssetBriefViewModel() : base(true, "The Assest field is required.")
        {

        }
        [DisplayName("UOM")]
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
