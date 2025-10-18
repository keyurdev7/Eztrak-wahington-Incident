using Select2.Model;
using System.ComponentModel;

namespace ViewModels
{
    public class TechnicianBriefViewModel : BaseSelect2VM, ISelect2BaseVM
    {
        public TechnicianBriefViewModel() : base(false, "The Technician field is required.")
        {

        }
        public long Id { get; set; }
        [DisplayName("Name")]
        public string? Name { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public override string? Select2Text
        {
            get
            {
                return Name;
            }
        }


    }

}
