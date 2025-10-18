using Select2.Model;
using System.ComponentModel;

namespace ViewModels.Administrator
{
    public class AdministratorSelect2ViewModel : BaseSelect2VM
    {
        public AdministratorSelect2ViewModel() : base(true, "The Administrator field is required.")
        {

        }
        [DisplayName("Name")]
        public override string? Select2Text { get; set; }
    }
}
