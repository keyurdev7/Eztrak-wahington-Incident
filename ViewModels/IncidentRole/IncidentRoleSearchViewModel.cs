// ViewModels/IncidentRoleSearchViewModel.cs
using Pagination;

namespace ViewModels
{
    public class IncidentRoleSearchViewModel : BaseSearchModel
    {
        public string? Name { get; set; }
        public string? Description { get; set; }

        public override string OrderByColumn { get; set; } = "Name";
    }
}
