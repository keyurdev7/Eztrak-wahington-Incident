using Pagination;

namespace ViewModels
{
    public class SourceSearchViewModel : BaseSearchModel
    {
        public string? Name { get; set; }
        public override string OrderByColumn { get; set; } = "Name";
    }
}
