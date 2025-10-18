using Helpers.Extensions;
using System.ComponentModel;
using ViewModels.Shared;

namespace ViewModels
{
    public class AssetTypeLevel1DetailViewModel : BaseCrudViewModel
    {
        public long Id { get; set; }
        [DisplayName("Asset Type Level 1")]
        public string Name { get; set; }
        public AssetTypeBriefViewModel AssetType { get; set; } = new();
        public List<AssetTypeLevel2DetailViewModel> AssetTypeLevel2 { get; set; } = new();

        public DateTime CreatedOn { get; set; }
    }
}
