using Select2.Model;
using System.ComponentModel;

namespace ViewModels.Guest
{
    public class GuestBriefViewModel
    {
        public long Id { get; set; }
        [DisplayName("Name")]
        public string Name { get; set; }
    }

}
