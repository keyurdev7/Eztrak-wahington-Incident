using DocumentFormat.OpenXml.Wordprocessing;
using Enums;
using Models.Models.Shared;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class TicketHistory : BaseDBModel
    {
        [Display(Name = "Ticket Number")]
        public string TicketNo { get; set; }
        public string Description { get; set; }
        public string? ITResponse { get; set; }
        public TicketCategory Category { get; set; }
        public TicketStatus Status { get; set; }
        public string? ShopperNote { get; set; }
        public DateTime? SubmittedOn { get; set; }

        [ForeignKey("Ticket")]
        public long TicketId { get; set; }
        public Ticket Ticket { get; set; }

        [ForeignKey("Shopper")]
        public long ShopperId { get; set; }
        public ApplicationUser Shopper { get; set; }
    }
}
