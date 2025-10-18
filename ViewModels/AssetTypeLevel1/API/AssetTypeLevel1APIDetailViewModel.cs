using System.ComponentModel;

namespace ViewModels
{
    public class AssetTypeLevel1DetailAPIViewModel
    {
        public long Id { get; set; }
        [DisplayName("Asset Type Level 1")]
        public string Name { get; set; }
        public long SelectedAssetTypeLevel2Id { get; set; }
        public List<AssetTypeLevel2DetailAPIViewModel>? AssetTypeLevel2 { get; set; } = new();
    }
}
