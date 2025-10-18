using System.ComponentModel.DataAnnotations;
using Enums;
using Microsoft.AspNetCore.Http;

namespace ViewModels
{
    public class WorkOrderModifyStatusAPIViewModel
    {
        public long Id { get; set; }
        [Required]
        public new WOStatusCatalog Status { get; set; }
        public string? Comment { get; set; }
        public List<IFormFile>? Images { get; set; } = new();
    }
}
