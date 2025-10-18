using System.ComponentModel.DataAnnotations;

namespace ViewModels
{
    public class AssetAssociationDetailAPIViewModel
    {
        public long Id { get; set; }
        [Range(1, long.MaxValue)]
        public long AssetId { get; set; }
        [Range(1, long.MaxValue)]
        public long AssetTypeId { get; set; }

        public AssetTypeLevel1DetailAPIViewModel AssetTypeLevel1 { get; set; } = new();
    }
}
