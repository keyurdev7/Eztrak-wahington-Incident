using System.ComponentModel.DataAnnotations;
using Models.Common.Interfaces;
using ViewModels.Shared;
using Enums;
using Microsoft.AspNetCore.Http;

namespace ViewModels
{
        
    public class TicketStatusUpdateViewModel : BaseUpdateVM, IBaseCrudViewModel, IIdentitifier
    {
        public long Id { get; set; }
        [Display(Name = "IT Response", Prompt = "IT Response")]
        public string ITResponse { get; set; }
        public TicketStatus Status { get; set; }
        
    }
}
