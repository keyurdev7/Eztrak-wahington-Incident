using Select2.Model;
using System.ComponentModel;

namespace ViewModels.Manager
{
    public class ManagerBriefViewModel : BaseSelect2VM, ISelect2BaseVM
    {
        public ManagerBriefViewModel() : base(false, "The Manager field is required.")
        {

        }
        public long Id { get; set; }
        [DisplayName("Name")]
        public string? Name { get; set; }
        public string? Telephone { get; set; }
        public string? Email { get; set; }
        public override string? Select2Text
        {
            get
            {
                return Name;
            }
        }


    }

}
