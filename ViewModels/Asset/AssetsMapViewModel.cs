namespace ViewModels.Asset
{
    public class AssetsMapViewModel
    {
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public long AssetId { get; set; }

        public string FormattedAssetId
        {
            get
            {
                return AssetId.ToString().PadLeft(4, '0');
            }
        }
        public List<AssetDetailViewModel> Assets { get; set; } = new();

    }
}
