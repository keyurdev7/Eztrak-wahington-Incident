using System.ComponentModel;

namespace ViewModels
{
    public class UOMBriefViewModel : BaseSelect2VM, ISelect2BaseVM
    {
        public UOMBriefViewModel() : base(true, "The UOM field is required.")
        {

        }
        public UOMBriefViewModel(bool isValidationEnabled, string errorMessage) : base(isValidationEnabled, errorMessage)
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
