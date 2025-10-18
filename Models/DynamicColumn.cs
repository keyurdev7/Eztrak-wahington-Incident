using Enums;
using Models.Models.Shared;

namespace Models
{
    public class DynamicColumn : BaseDBModel
    {
        public string Title { get; set; }
        public string Name { get; set; }
        public string? Regex { get; set; }
        public DynamicColumnType Type { get; set; }
        public DynamicColumnEntityType EntityType { get; set; }

        public List<DynamicColumnOption>? Options { get; set; }
        public List<DynamicColumnValue>? Values { get; set; }
    }
}
