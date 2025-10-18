using Select2.Model;
using System.ComponentModel;

namespace ViewModels
{
    public class ReplaceBriefViewModel : BaseSelect2VM, ISelect2BaseVM
    {
        public ReplaceBriefViewModel() : base(false, "The Replace field is required.")
        {

        }
        [DisplayName("Replace")]
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
