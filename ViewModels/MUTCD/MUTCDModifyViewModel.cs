using System.ComponentModel.DataAnnotations;
using Models.Common.Interfaces;
using ViewModels.Shared;
using Enums;
using ViewModels;
using Helpers.File;
using Microsoft.AspNetCore.Http;

namespace ViewModels
{
    public class MUTCDModifyViewModel : BaseUpdateVM, IBaseCrudViewModel, IIdentitifier, IFileModel
    {
        [Display(Name = "Code", Prompt = "Code")]
        public string Code { get; set; }
        [Display(Name = "Description", Prompt = "Description")]
        public string Description { get; set; }
        public string? ImageUrl { get; set; }
        public IFormFile? File { get; set; }
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
    }
}
