using Pagination;

namespace ViewModels
{
    public class SupplierSearchViewModel : BaseSearchModel
    {
        public string? Name { get; set; }
        public override string OrderByColumn { get; set; } = "Name";
    }
}
