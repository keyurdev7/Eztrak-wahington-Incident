using Select2.Model;
using System.ComponentModel;

namespace ViewModels
{
    public class MUTCDBriefViewModel : BaseSelect2VM, ISelect2BaseVM
    {
        public MUTCDBriefViewModel() : base(false, "")
        {

        }
        public MUTCDBriefViewModel(bool isValidationEnabled, string errorMessage) : base(isValidationEnabled, errorMessage)
        {

        }
        [DisplayName("MUTCD")]
        public string? Name { get => Code; }
        public string? Code { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }

        public override string? Select2Text
        {
            get
            {
                return Code + "-" + Description;
            }
        }
    }

}
