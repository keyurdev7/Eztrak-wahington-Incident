using Select2.Model;
using System.ComponentModel;

namespace ViewModels
{
    public class RepairBriefViewModel : BaseSelect2VM, ISelect2BaseVM
    {
        public RepairBriefViewModel() : base(false, "The Repair field is required.")
        {

        }
        [DisplayName("Repair")]
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
