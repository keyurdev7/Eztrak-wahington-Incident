using Helpers.Extensions;
using ViewModels.Shared;

namespace ViewModels
{
    public class AssetTypeLevel2DetailViewModel : BaseCrudViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string AssetTypeLevel1Name { get; set; }

        public bool IsChecked { get; set; }

        public AssetTypeLevel1BriefViewModel AssetTypeLevel1 { get; set; } = new();
        public AssetTypeBriefViewModel AssetType { get; set; } = new();

        public DateTime CreatedOn { get; set; }
    }
}
