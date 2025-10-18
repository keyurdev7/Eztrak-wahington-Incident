using Pagination;

namespace ViewModels
{
    public class AssetTypeLevel1SearchViewModel : BaseSearchModel
    {
        public string? Name { get; set; }
        public AssetTypeBriefViewModel AssetType { get; set; } = new();
    }
}
