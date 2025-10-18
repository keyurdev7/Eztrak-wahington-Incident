using ViewModels.Shared;

namespace ViewModels
{
    public class AssetMapViewModel : BaseCrudViewModel
    {
        public long Id { get; set; }
        public long AssetId { get; set; }
        public string Description { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string AssetClass { get; set; }

        public string AssetType { get; set; }
        public string Condition { get; set; }
        public string PoleId { get; set; }

        public Dictionary<string, string> AssetsTypeSubLevels { get; set; } = new();

        public List<MapAssetAssociationDetailViewModel> AssetAssociations { get; set; } = new();


    }
    public class MapAssetAssociationDetailViewModel
    {
        public MapBriefVM Asset { get; set; } = new();

        public MapBriefVM AssetType { get; set; } = new();

        public MapBriefVM AssetTypeLevel1 { get; set; } = new();

        public MapBriefVM AssetTypeLevel2 { get; set; } = new();
        public long Id { get; set; }
    }

    public class MapBriefVM
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }

}
