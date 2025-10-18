using Select2.Model;
using System.ComponentModel;

namespace ViewModels
{
    public class StreetServiceRequestBriefViewModel : BaseSelect2VM, ISelect2BaseVM
    {
        public StreetServiceRequestBriefViewModel() : base(true, "The Street Service Request field is required.")
        {

        }
        [DisplayName("Street Service Request")]
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
