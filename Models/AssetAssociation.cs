using System;
using System.ComponentModel.DataAnnotations.Schema;
using Models.Models.Shared;

namespace Models
{
    public class AssetAssociation : BaseDBModel
    {
        [ForeignKey("Asset")]
        public long AssetId { get; set; }
        public Asset Asset { get; set; }

        [ForeignKey("AssetType")]
        public long AssetTypeId { get; set; }
        public AssetType AssetType { get; set; }

        [ForeignKey("AssetTypeLevel1")]
        public long AssetTypeLevel1Id { get; set; }
        public AssetTypeLevel1 AssetTypeLevel1 { get; set; }

        [ForeignKey("AssetTypeLevel2")]
        public long AssetTypeLevel2Id { get; set; }
        public AssetTypeLevel2 AssetTypeLevel2 { get; set; }
    }
}

