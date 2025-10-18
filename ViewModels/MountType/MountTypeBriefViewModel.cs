using Select2.Model;
using System.ComponentModel;

namespace ViewModels
{
    public class MountTypeBriefViewModel : BaseSelect2VM, ISelect2BaseVM
    {
        public MountTypeBriefViewModel() : base(false, "")
        {

        }
        [DisplayName("MountType")]
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
