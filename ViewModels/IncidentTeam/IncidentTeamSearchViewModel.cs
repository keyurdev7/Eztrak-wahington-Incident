using Pagination;

namespace ViewModels
{
    public class IncidentTeamSearchViewModel : BaseSearchModel
    {
        public string? Name { get; set; }
        public string? Department { get; set; }
        public string? Contact { get; set; }

        // Optional: search by specialization text
        public string? Specialization { get; set; }

        public override string OrderByColumn { get; set; } = "Name";
    }
}
