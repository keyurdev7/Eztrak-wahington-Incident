using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using ViewModels.Shared;

namespace ViewModels.Permission
{
    public class PermissionCreateViewModel : BaseCreateVM, IBaseCrudViewModel
    {
        [Required]
        [MaxLength(200)]
        [DisplayName("Name")]
        public string Name { get; set; }
        public string Screen { get; set; }
    }
}
