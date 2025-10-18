using Select2.Model;
using System.ComponentModel;

namespace ViewModels.Example
{
    public class ExampleBriefViewModel : BaseSelect2VM, ISelect2BaseVM
    {
        public ExampleBriefViewModel() : base(true, "The Example field is required.")
        {

        }
        [DisplayName("Example")]
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
