using System.ComponentModel;

namespace ViewModels
{
    public class DynamicColumnOptionBriefViewModel : BaseSelect2VM, ISelect2BaseVM
    {
        public DynamicColumnOptionBriefViewModel() : base(false)
        {

        }
        [DisplayName("DynamicColumn")]
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
