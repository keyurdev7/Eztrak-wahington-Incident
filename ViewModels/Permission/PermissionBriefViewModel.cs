using Select2.Model;
using System.ComponentModel;

namespace ViewModels.Permission
{
    public class PermissionBriefViewModel : BaseSelect2VM, ISelect2BaseVM
    {
        public PermissionBriefViewModel() : base(true, "The Permission field is required.")
        {

        }
        [DisplayName("Permission")]
        public string Name { get; set; }
        public string Screen { get; set; }

        public override string Select2Text
        {
            get
            {
                return Name;
            }
        }
    }

}
