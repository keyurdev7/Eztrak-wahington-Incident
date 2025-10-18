using DocumentFormat.OpenXml.Wordprocessing;
using Enums;
using Models.Models.Shared;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class Ticket : BaseDBModel
    {
        [Display(Name = "Ticket Number")]
        public string TicketNo { get; set; }
        public string Description { get; set; }
        public string? ITResponse { get; set; }
        public TicketCategory Category { get; set; }
        public TicketStatus Status { get; set; }
    }
}
