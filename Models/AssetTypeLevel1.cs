using Models.Models.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class AssetTypeLevel1 : BaseDBModel
    {
        public string Name { get; set; }
        [ForeignKey("AssetType")]
        public long AssetTypeId { get; set; }
        public AssetType AssetType { get; set; }

        public List<AssetTypeLevel2> AssetTypeLevel2 { get; set; } = new();

    }
}
