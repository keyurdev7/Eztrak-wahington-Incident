using Helpers.Datetime;
using Helpers.File;
using Microsoft.AspNetCore.Http;
using Models.Common.Interfaces;
using System.ComponentModel.DataAnnotations;
using ViewModels.Shared;
using ViewModels.Shared.Notes;

namespace ViewModels
{
    public class WorkOrderNotesViewModel : BaseUpdateVM, IBaseCrudViewModel, IIdentitifier, IFileModel, INotesViewModel
    {
        [Display(Prompt = "Description")]
        public string Description { get; set; }
        public string? FileUrl { get; set; }
        public IFormFile File { get; set; }
        public string? Type { get; set; }
        public string GetBaseFolder()
        {
            var ext = Type;
            if (ext == ".jpg" || ext == ".jpeg" || ext == ".png")
            {
                return "Images";
            }
            if (ext == ".mp4")
            {
                return "Videos";
            }
            if (ext == ".pdf" || ext == ".docx" || ext == ".xlsx" || ext == ".txt")
            {
                return "Documents";
            }
            return "Others";
        }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string FormattedCreatedOn
        {
            get
            {
                return CreatedOn.FormatDateInPST();
            }
        }
        public long WorkOrderId { get; set; }
    }
}
