using Models.Models.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class StreetServiceRequestNotes : BaseDBModel
    {
        public string Description { get; set; }
        public string? FileUrl { get; set; }

        [ForeignKey("StreetServiceRequest")]
        public long StreetServiceRequestId { get; set; }
        public StreetServiceRequest StreetServiceRequest { get; set; }
    }
}
