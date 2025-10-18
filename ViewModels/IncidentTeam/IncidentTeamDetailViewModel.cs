using Helpers.Extensions;
using ViewModels.Shared;
using System.Collections.Generic;

namespace ViewModels
{
    public class IncidentTeamDetailViewModel : BaseCrudViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public string? Department { get; set; }
        public string? Contact { get; set; }

        // CSV persisted value (raw)
        public string? Specializations { get; set; }

        // Splitted list helpful for rendering as pills in the UI
        public List<string>? SpecializationList { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
