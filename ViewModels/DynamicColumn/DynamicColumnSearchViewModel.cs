using Enums;
using Pagination;

namespace ViewModels
{
    public class DynamicColumnSearchViewModel : BaseSearchModel
    {
        public string? Name { get; set; }
        public DynamicColumnType? Type { get; set; }
        public DynamicColumnEntityType? EntityType { get; set; }
        public override string OrderByColumn { get; set; } = "Name";
    }
}
