using Pagination;

namespace ViewModels
{
    public class PolicySearchViewModel : BaseSearchModel
    {
        public string? Name { get; set; }
        public string? Description { get; set; }

        public override string OrderByColumn { get; set; } = "Name";
    }
}
