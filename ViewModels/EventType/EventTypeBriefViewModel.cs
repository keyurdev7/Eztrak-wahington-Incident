using Select2.Model;
using System.ComponentModel;

namespace ViewModels
{
    public class EventTypeBriefViewModel : BaseSelect2VM, ISelect2BaseVM
    {
        public EventTypeBriefViewModel() : base(true, "The Event Type field is required.")
        {
        }

        public EventTypeBriefViewModel(bool isValidationEnabled, string errorMessage)
            : base(isValidationEnabled, errorMessage)
        {
        }

        [DisplayName("Event Type")]
        public string? Name { get; set; }

        [DisplayName("Description")]
        public string? Description { get; set; }

        public override string? Select2Text
        {
            get
            {
                return !string.IsNullOrEmpty(Name) ? $"{Name} - {Description}" : Name;
            }
        }
    }
}
