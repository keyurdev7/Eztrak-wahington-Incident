using Select2.Model;
using System.ComponentModel;

namespace ViewModels.Users
{
    public class UserBriefViewModel : BaseSelect2VM, ISelect2BaseVM
    {
        [DisplayName("User")]
        public string? Name { get => string.IsNullOrEmpty(FirstName) && string.IsNullOrEmpty(LastName) ? string.IsNullOrEmpty(Username) ? "" : $"{Username}" : $"{FirstName} {LastName}"; }

        public override string? Select2Text
        {
            get
            {
                return Name;
            }
        }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Username { get; set; }
    }

}
