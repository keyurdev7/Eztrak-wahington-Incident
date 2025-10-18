using System.ComponentModel.DataAnnotations;
using Models.Common.Interfaces;
using ViewModels.Shared;
using Enums;
using ViewModels;

namespace ViewModels
{
    public class CraftSkillModifyViewModel : BaseUpdateVM, IBaseCrudViewModel, IIdentitifier
    {
        [Display(Name = "Name", Prompt = "Name")]
        public string Name { get; set; }
        [Display(Name = "ST Rate", Prompt = "ST Rate")]

        public double STRate { get; set; }
        [Display(Name = "OT Rate", Prompt = "OT Rate")]

        public double OTRate { get; set; }
        [Display(Name = "DT Rate", Prompt = "DT Rate")]

        public double DTRate { get; set; }
    }
}
