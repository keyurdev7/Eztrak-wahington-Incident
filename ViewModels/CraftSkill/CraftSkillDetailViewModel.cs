using Helpers.Extensions;
using System.ComponentModel.DataAnnotations;
using ViewModels.Shared;

namespace ViewModels
{
    public class CraftSkillDetailViewModel : BaseCrudViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        [Display(Name = "ST Rate", Prompt = "ST Rate")]

        public double STRate { get; set; }
        [Display(Name = "OT Rate", Prompt = "OT Rate")]

        public double OTRate { get; set; }
        [Display(Name = "DT Rate", Prompt = "DT Rate")]

        public double DTRate { get; set; }
    }
}
