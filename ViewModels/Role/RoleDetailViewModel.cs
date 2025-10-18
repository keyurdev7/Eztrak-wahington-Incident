using Select2.Model;
using System.ComponentModel;
using ViewModels.Shared;

namespace ViewModels.Role
{
    public class RoleDetailViewModel : BaseCrudViewModel
    {
        public long Id { get; set; }
        [DisplayName("Name")]
        public string Name { get; set; }

    }
}
