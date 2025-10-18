using Select2.Model;
using System.ComponentModel;

namespace ViewModels
{
    public class LocationBriefViewModel : BaseSelect2VM, ISelect2BaseVM
    {
        public LocationBriefViewModel() : base(true, "The Location field is required.")
        {

        }
        public LocationBriefViewModel(bool isValidationEnabled, string errorMessage) : base(isValidationEnabled, errorMessage)
        {

        }
        [DisplayName("Location")]
        public string Name { get; set; }

        public override string? Select2Text
        {
            get
            {
                return Name;
            }
        }
    }

}
