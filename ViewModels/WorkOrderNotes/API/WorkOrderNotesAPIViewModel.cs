using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ViewModels
{
    public class WorkOrderNotesAPIViewModel
    {
        [Display(Prompt = "Description")]
        public string Description { get; set; }
        public IFormFile? File { get; set; }
        public long WorkOrderId { get; set; }
    }
}
