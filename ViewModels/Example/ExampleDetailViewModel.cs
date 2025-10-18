using Select2.Model;
using System.ComponentModel;
using ViewModels.Shared;

namespace ViewModels.Example
{
    public class ExampleDetailViewModel : BaseCrudViewModel
    {
        public long Id { get; set; }
        [DisplayName("Name")]
        public string Name { get; set; }

    }
}
