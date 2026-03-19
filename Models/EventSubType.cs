using Models.Models.Shared;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class EventSubType : BaseDBModel
    {
        [Required]
        [ForeignKey("EventType")]
        public long EventTypeId { get; set; }
        public EventType EventType { get; set; }

        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }
    }
}

