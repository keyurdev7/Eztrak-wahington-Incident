using Select2.Model;
using System.ComponentModel;

namespace ViewModels
{
    public class ContractorBriefViewModel : BaseSelect2VM, ISelect2BaseVM
    {
        public ContractorBriefViewModel() : base(true, "The Contractor field is required.")
        {

        }
        [DisplayName("Contractor")]
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
