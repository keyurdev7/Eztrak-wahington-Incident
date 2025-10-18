using Select2.Model;
using System.ComponentModel;

namespace ViewModels.Administrator
{
    public class AdministratorBriefViewModel
    {
        public long Id { get; set; }
        [DisplayName("Name")]
        public string Name { get; set; }
    }

}
