using Select2.Model;
using System.ComponentModel;

namespace ViewModels.Manager
{
    public class ManagerSelect2ViewModel : BaseSelect2VM
    {
        public ManagerSelect2ViewModel() : base(true, "The Manager field is required.")
        {

        }
        [DisplayName("Name")]
        public override string? Select2Text { get; set; }
    }
}
