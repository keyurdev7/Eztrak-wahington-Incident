using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using ViewModels.Shared;

namespace ViewModels.Role
{
    public class RoleCreateViewModel : BaseCreateVM, IBaseCrudViewModel
    {
        [Required]
        [MaxLength(200)]
        [DisplayName("Name")]
        public string Name { get; set; }
        public string? NormalizedName { get => Name.ToUpper(); }
    }
}
