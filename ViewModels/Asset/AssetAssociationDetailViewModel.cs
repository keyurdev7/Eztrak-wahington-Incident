namespace ViewModels
{
    public class AssetAssociationDetailViewModel
    {
        public long Id { get; set; }

        public AssetBriefViewModel Asset { get; set; } = new();

        public AssetTypeBriefViewModel AssetType { get; set; } = new();

        public AssetTypeLevel1BriefViewModel AssetTypeLevel1 { get; set; } = new();

        public AssetTypeLevel2BriefViewModel AssetTypeLevel2 { get; set; } = new();
    }
}
