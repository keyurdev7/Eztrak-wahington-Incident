using Models.Models.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class AssetTypeLevel2 : BaseDBModel
    {
        public string Name { get; set; }
        [ForeignKey("AssetTypeLevel1")]
        public long AssetTypeLevel1Id { get; set; }
        public AssetTypeLevel1 AssetTypeLevel1 { get; set; }
        [ForeignKey("AssetType")]
        public long AssetTypeId { get; set; }
        public AssetType AssetType { get; set; }
        
    }
}
