using DocumentFormat.OpenXml.Wordprocessing;
using Enums;
using Pagination;
using System.ComponentModel.DataAnnotations;

namespace ViewModels
{
    public class TicketSearchViewModel : BaseSearchModel
    {
        [Display(Name = "Ticket Number")]
        public string TicketNo { get; set; }
        public TicketStatus? Status { get; set; }
        public string Description { get; set; }


    }
}
