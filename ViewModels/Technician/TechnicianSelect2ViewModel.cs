using Select2.Model;
using System.ComponentModel;

namespace ViewModels.Technician
{
    public class TechnicianSelect2ViewModel : BaseSelect2VM
    {
        public TechnicianSelect2ViewModel() : base(true, "The Technician field is required.")
        {

        }
        [DisplayName("Name")]
        public override string? Select2Text { get; set; }
    }
}
