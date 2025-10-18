using Pagination;

namespace ViewModels
{
    public class AssetTypeLevel2SearchViewModel : BaseSearchModel
    {
        public string? Name { get; set; }
        public AssetTypeLevel1BriefViewModel AssetTypeLevel1 { get; set; } = new();
        public AssetTypeBriefViewModel AssetType { get; set; } = new();
    }
}
