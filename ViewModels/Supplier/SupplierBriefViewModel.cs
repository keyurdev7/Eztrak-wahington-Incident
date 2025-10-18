using Select2.Model;
using System.ComponentModel;

namespace ViewModels
{
    public class SupplierBriefViewModel : BaseSelect2VM, ISelect2BaseVM
    {
        public SupplierBriefViewModel() : base(true, "The Supplier field is required.")
        {

        }
        public SupplierBriefViewModel(bool isValidationEnabled, string errorMessage) : base(isValidationEnabled, errorMessage)
        {

        }
        [DisplayName("Supplier")]
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
