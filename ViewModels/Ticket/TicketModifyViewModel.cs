using System.ComponentModel.DataAnnotations;
using Models.Common.Interfaces;
using ViewModels.Shared;
using Enums;
using Microsoft.AspNetCore.Http;

namespace ViewModels
{
    public class TicketModifyViewModel : BaseUpdateVM, IBaseCrudViewModel, IIdentitifier
    {
        [Display(Name = "Ticket Number")]
        public string? TicketNo { get; set; }
        public string Description { get; set; }
        [Display(Name = "IT Response")]
        public string? ITResponse { get; set; }
        public TicketCategory Category { get; set; }
        public TicketStatus Status { get; set; } = TicketStatus.Open;
        public List<AttachmentVM> ImagesList { get; set; } = new();
        public List<IFormFile> Images { get; set; } = new();
    }
}
