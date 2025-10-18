using Select2.Model;
using System.ComponentModel;

namespace ViewModels.Guest
{
    public class GuestSelect2ViewModel : BaseSelect2VM
    {
        public GuestSelect2ViewModel() : base(true, "The Guest field is required.")
        {

        }
        [DisplayName("Name")]
        public override string? Select2Text { get; set; }
    }
}
