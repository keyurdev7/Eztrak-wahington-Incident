using Enums;
using Pagination;

namespace ViewModels
{
    public class UserSearchSettingSearchViewModel : BaseSearchModel
    {
        public string? Name { get; set; }
        public SearchFilterTypeCatalog? Type { get; set; }
    }
}
