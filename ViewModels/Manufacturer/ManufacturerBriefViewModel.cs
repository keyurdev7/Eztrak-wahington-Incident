using Select2.Model;
using System.ComponentModel;

namespace ViewModels
{
    public class ManufacturerBriefViewModel : BaseSelect2VM, ISelect2BaseVM
    {
        public ManufacturerBriefViewModel() : base(true, "The Manufacturer field is required.")
        {

        }
        public ManufacturerBriefViewModel(bool isValidationEnabled,string errorMessage) : base(isValidationEnabled, errorMessage)
        {

        }
        [DisplayName("Manufacturer")]
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
