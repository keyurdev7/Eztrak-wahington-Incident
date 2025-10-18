using Pagination;

namespace ViewModels
{
    public class StatusLegendSearchViewModel : BaseSearchModel
    {
        public string? Name { get; set; }
        public string? Description { get; set; }   // <- added
        public string? Color { get; set; }

        public override string OrderByColumn { get; set; } = "Name";
    }
}
