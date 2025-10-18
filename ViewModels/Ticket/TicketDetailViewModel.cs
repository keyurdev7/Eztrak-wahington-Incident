using DocumentFormat.OpenXml.Wordprocessing;
using Enums;
using Helpers.Datetime;
using Helpers.Extensions;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using ViewModels.Shared;

namespace ViewModels
{
    public class TicketDetailViewModel : BaseCrudViewModel
    {
        public long Id { get; set; }
        [Display(Name = "Ticket Number")]
        public string TicketNo { get; set; }
        public string Description { get; set; }
        [Display(Name = "IT Response")]
        public string? ITResponse { get; set; }
        public TicketCategory Category { get; set; }
        public string FormattedCategory { get => Category.GetDisplayName(); }
        public TicketStatus Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public string FormattedCreatedOn { get => CreatedOn.FormatDate(); }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public List<AttachmentVM> ImagesList { get; set; } = new();
        public List<IFormFile> Images { get; set; } = new();
    }
}
