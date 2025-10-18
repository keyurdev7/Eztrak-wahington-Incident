using Enums;
using Pagination;

namespace ViewModels
{
    public class DynamicColumnValueSearchViewModel : BaseSearchModel
    {
        public string? Name { get; set; }
        public DynamicColumnEntityType? EntityType { get; set; }
        public override string OrderByColumn { get; set; } = "Name";

    }
}
