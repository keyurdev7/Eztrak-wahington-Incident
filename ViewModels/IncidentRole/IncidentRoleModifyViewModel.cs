// ViewModels/IncidentRoleModifyViewModel.cs
using System.ComponentModel.DataAnnotations;
using Models.Common.Interfaces;
using ViewModels.Shared;
using Enums;

namespace ViewModels
{
    public class IncidentRoleModifyViewModel : BaseUpdateVM, IBaseCrudViewModel, IIdentitifier
    {
        [Display(Name = "Name", Prompt = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [Display(Name = "Description", Prompt = "Description")]
        public string? Description { get; set; }

        // Id property is inherited from IIdentitifier/BaseUpdateVM typically
    }
}
