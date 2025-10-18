using Pagination;
using System.ComponentModel;

namespace ViewModels.Users
{
    public class UserSearchViewModel : BaseSearchModel
    {
        [DisplayName("Name")]
        public string FirstName { get; set; }
        [DisplayName("Last Name")]
        public string LastName { get; set; }
        public string Email { get; set; }
        public override string OrderByColumn { get; set; } = "FirstName";

    }
}
