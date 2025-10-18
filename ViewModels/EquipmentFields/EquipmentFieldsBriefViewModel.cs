using Select2.Model;
using System.ComponentModel;

namespace ViewModels
{
    public class EquipmentFieldsBriefViewModel : BaseSelect2VM, ISelect2BaseVM
    {
        public EquipmentFieldsBriefViewModel() : base(true, "The Name field is required.")
        {

        }
        public EquipmentFieldsBriefViewModel(bool isValidationEnabled, string errorMessage) : base(isValidationEnabled, errorMessage)
        {

        }
        [DisplayName("Name")]
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