using System.ComponentModel.DataAnnotations;
using Models.Common.Interfaces;
using ViewModels.Shared;
using Enums;

namespace ViewModels
{
    public class PolicyModifyViewModel : BaseUpdateVM, IBaseCrudViewModel, IIdentitifier
    {
        [Display(Name = "Name", Prompt = "Name")]
        public string Name { get; set; }

        [Display(Name = "Description", Prompt = "Description")]
        public string? Description { get; set; }
        public List<string> PolicySteps { get; set; }
    }
}