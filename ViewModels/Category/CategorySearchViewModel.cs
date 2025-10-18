using Pagination;

namespace ViewModels
{
    public class CategorySearchViewModel : BaseSearchModel
    {
        public string? Name { get; set; }
        public override string OrderByColumn { get; set; } = "Name";
    }
}
