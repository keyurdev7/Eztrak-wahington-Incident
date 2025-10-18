using Select2.Model;
using System.ComponentModel;

namespace ViewModels.Employee
{
    public class EmployeeBriefViewModel
    {
        public long Id { get; set; }
        [DisplayName("Name")]
        public string Name { get; set; }
    }

}
