using Select2.Model;
using System.ComponentModel;

namespace ViewModels.Employee
{
    public class EmployeeSelect2ViewModel : BaseSelect2VM
    {
        public EmployeeSelect2ViewModel() : base(true, "The Employee field is required.")
        {

        }
        [DisplayName("Name")]
        public override string? Select2Text { get; set; }
    }
}
