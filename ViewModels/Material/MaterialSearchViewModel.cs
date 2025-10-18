using Pagination;

namespace ViewModels
{
    public class MaterialSearchViewModel : BaseSearchModel
    {
        public string? MaterialID { get; set; }
        public string? Name { get; set; }
        public string? Category { get; set; }
        public string? Unit { get; set; }

        public override string OrderByColumn { get; set; } = "Name";
    }
}
