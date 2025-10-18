using Enums;
using System.ComponentModel.DataAnnotations;

namespace Models.Models.Shared
{
    public class Attachment : BaseDBModel
    {
        [StringLength(100)]
        public string? Name { get; set; }
        [StringLength(50)]
        public string? Url { get; set; }
        [StringLength(50)]
        public string? Type { get; set; }
        [StringLength(100)]
        public string? Title { get; set; }
        [StringLength(50)]
        public string? Size { get; set; }
        public long? EntityId { get; set; }
        public AttachmentEntityType? EntityType { get; set; }
        
    }
}
