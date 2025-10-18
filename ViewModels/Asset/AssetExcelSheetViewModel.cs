namespace ViewModels
{
    public class AssetExcelSheetViewModel
    {
        public string PoleId { get; set; }
        public string MUTCD { get; set; }
        public long MUTCDId { get; set; }
        public string MountType { get; set; }
        public string SignMountType { get; set; }
        public long MountTypeId { get; set; }
        public string StreetName { get; set; }
        public string AssetId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string SignPost { get; set; }
        public string SignMount { get; set; }
        public string SignHardware { get; set; }
        public string SignCondition { get; set; }
        public string SignDimension { get; set; }
        public string AssetType { get; set; }
        public string AssetClass { get; set; }
    }
}
