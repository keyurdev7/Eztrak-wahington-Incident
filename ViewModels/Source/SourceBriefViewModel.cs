using Select2.Model;
using System.ComponentModel;

namespace ViewModels
{
    public class SourceBriefViewModel : BaseSelect2VM, ISelect2BaseVM
    {
        public SourceBriefViewModel() : base(false, "The Source field is required.")
        {

        }
        [DisplayName("Source")]
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
