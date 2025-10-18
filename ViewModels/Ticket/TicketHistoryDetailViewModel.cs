using Enums;
using Helpers.Datetime;
using Helpers.Extensions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModels.Shared;

namespace ViewModels.Ticket
{
    public class TicketHistoryDetailViewModel : BaseCrudViewModel
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
        public DateTime SubmittedOn { get; set; }
        public string FormattedSubmittedOn { get => SubmittedOn.FormatDate(); }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public long TicketId { get; set; }
    }
}
