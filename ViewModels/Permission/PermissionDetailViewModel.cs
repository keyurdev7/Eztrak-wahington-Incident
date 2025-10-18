using Select2.Model;
using System.ComponentModel;
using ViewModels.Shared;

namespace ViewModels.Permission
{
    public class PermissionDetailViewModel : BaseCrudViewModel
    {
        public long Id { get; set; }
        [DisplayName("Name")]
        public string Name { get; set; }
        public string Screen { get; set; }
    }
}
