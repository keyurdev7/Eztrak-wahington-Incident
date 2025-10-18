using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using ViewModels.Shared;

namespace ViewModels.Example
{
    public class ExampleCreateViewModel : BaseCreateVM, IBaseCrudViewModel
    {
        [Required]
        [MaxLength(200)]
        [DisplayName("Name")]
        public string Name { get; set; }
    }
}
