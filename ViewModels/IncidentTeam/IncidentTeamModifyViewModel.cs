using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Models.Common.Interfaces;
using ViewModels.Shared;

namespace ViewModels
{
    public class IncidentTeamModifyViewModel : BaseUpdateVM, IBaseCrudViewModel, IIdentitifier
    {
        [Display(Name = "Team", Prompt = "Team name")]
        public string Name { get; set; }

        [Display(Name = "Department", Prompt = "Select Department")]
        public string? Department { get; set; }   // static dropdown in UI

        [RegularExpression(@"^\d{3}-[A-Za-z]+-\d{3}$",
      ErrorMessage = "Contact ID must follow the format: 555-ABC-001")]
        [Display(Name = "Contact", Prompt = "Contact ID (e.g., 555-EMERGENCY-001)")]
        public string? Contact { get; set; }

        // Posted from the form as SpecializationList[0], SpecializationList[1], ...
        public List<string>? SpecializationList { get; set; }

        // CSV representation persisted to DB; service will convert list <-> csv
        [Display(Name = "Specializations", Prompt = "Enter specializations (comma separated)")]
        public string? Specializations { get; set; }
    }
}
