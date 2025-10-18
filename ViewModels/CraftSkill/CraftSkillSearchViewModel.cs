using Pagination;

namespace ViewModels
{
    public class CraftSkillSearchViewModel : BaseSearchModel
    {
        public string? Name { get; set; }
        public override string OrderByColumn { get; set; } = "Name";
    }
}
