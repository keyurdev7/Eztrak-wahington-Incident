using ViewModels;

namespace ViewModels.Authentication
{
    public class UserBriefVM : BaseSelect2VM
    {
        public UserBriefVM()
        {
        }
        public UserBriefVM(bool isValidationEnabled) : base(isValidationEnabled)
        {
        }
        public UserBriefVM(bool isValidationEnabled, string errorMessage) :
            base(isValidationEnabled, errorMessage)
        {
        }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        private string _name;
        public new string Name { get { return string.IsNullOrEmpty(_name) ? $"{FirstName} {LastName}" : _name; } set { _name = value; } }
        public string Role { get; set; }
    }
}
