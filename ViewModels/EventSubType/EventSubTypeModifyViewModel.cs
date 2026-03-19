using System.ComponentModel.DataAnnotations;
using Models.Common.Interfaces;
using ViewModels.Shared;

namespace ViewModels.EventSubType
{
    public class EventSubTypeModifyViewModel : BaseUpdateVM, IBaseCrudViewModel, IIdentitifier
    {
        [Required]
        public long EventTypeId { get; set; }

        public string? EventTypeName { get; set; }

        [Required]
        [Display(Name = "Name", Prompt = "Name")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Description", Prompt = "Description")]
        public string? Description { get; set; }
    }
}

