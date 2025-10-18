using DocumentFormat.OpenXml.Wordprocessing;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ViewModels
{
    public class TicketBriefViewModel : BaseSelect2VM, ISelect2BaseVM
    {
        public TicketBriefViewModel() : base(true, "The Ticket field is required.")
        {

        }
        public TicketBriefViewModel(bool isValidationEnabled, string errorMessage) : base(isValidationEnabled, errorMessage)
        {

        }
        [Display(Name = "Ticket Number")]
        public long TicketNo { get; set; }


    }

}
