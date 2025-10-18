using Models.Models.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class AssetNotes : BaseDBModel
    {
        public string Description { get; set; }
        public string? FileUrl { get; set; }

        [ForeignKey("Asset")]
        public long AssetId { get; set; }
        public Asset Asset { get; set; }
    }
}
