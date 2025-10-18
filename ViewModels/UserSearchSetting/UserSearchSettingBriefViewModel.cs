using System.ComponentModel;

namespace ViewModels
{
    public class UserSearchSettingBriefViewModel : BaseSelect2VM, ISelect2BaseVM
    {
        public UserSearchSettingBriefViewModel() : base(false)
        {

        }
        [DisplayName("Search Setting")]
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
