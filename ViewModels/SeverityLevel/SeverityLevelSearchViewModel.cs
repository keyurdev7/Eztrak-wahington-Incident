using Pagination;

namespace ViewModels
{
    public class SeverityLevelSearchViewModel : BaseSearchModel
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Color { get; set; }

        public override string OrderByColumn { get; set; } = "Name";
    }
}
