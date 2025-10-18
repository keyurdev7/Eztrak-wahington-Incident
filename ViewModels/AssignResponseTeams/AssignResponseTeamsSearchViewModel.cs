using Pagination;

namespace ViewModels
{
    public class AssignResponseTeamsSearchViewModel : BaseSearchModel
    {
        public string? Name { get; set; }
        public override string OrderByColumn { get; set; } = "Name";
    }
}