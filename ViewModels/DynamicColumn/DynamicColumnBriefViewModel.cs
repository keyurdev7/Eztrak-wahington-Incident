using System.ComponentModel;

namespace ViewModels
{
    public class DynamicColumnBriefViewModel : BaseSelect2VM, ISelect2BaseVM
    {
        public DynamicColumnBriefViewModel() : base(false)
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
