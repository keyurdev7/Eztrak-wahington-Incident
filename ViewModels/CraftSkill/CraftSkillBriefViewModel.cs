using Select2.Model;
using System.ComponentModel;

namespace ViewModels
{
    public class CraftSkillBriefViewModel : BaseSelect2VM, ISelect2BaseVM
    {
        public CraftSkillBriefViewModel() : base(false, "")
        {

        }
        [DisplayName("Craft Skill")]
        public string? Name { get; set; }
        public double STRate { get; set; }
        public double OTRate { get; set; }
        public double DTRate { get; set; }
        public override string? Select2Text
        {
            get
            {
                return Name;
            }
        }
    }

}
