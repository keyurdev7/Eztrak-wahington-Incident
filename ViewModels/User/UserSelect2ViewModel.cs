using Select2.Model;
using System.ComponentModel;

namespace ViewModels.Users
{
    public class UserSelect2ViewModel : BaseSelect2VM
    {
        public UserSelect2ViewModel() : base(false, "The User field is required.")
        {

        }
        [DisplayName("User")]
        public string Name { get => string.IsNullOrEmpty(FirstName) && string.IsNullOrEmpty(LastName) ? string.IsNullOrEmpty(Username) ? "" : $"{Username}" : $"{FirstName} {LastName}"; }

        public string Select2Text
        {
            get
            {
                return Name;
            }
        }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
    }

}
