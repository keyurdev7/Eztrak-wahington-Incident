using Enums;
using Helpers.Attributes;
using Helpers.File;
using Microsoft.AspNetCore.Http;
using Pagination;
using System.ComponentModel.DataAnnotations;

namespace ViewModels.Shared
{
    public class AttachmentVM : IFileModel
    {
        public long Id { get; set; }
        [DataType(DataType.Upload)]
        [MaxFileSize(25 * 1024 * 1024)]
        [AllowedExtensions(new string[] { ".jpg", ".pptx", ".png", ".jpeg", ".pdf", ".docx", ".xlsx" })]
        public IFormFile File { get; set; }
        public string? Url { get; set; }
        public string? Type { get; set; }
        [Display(Name = "Name", Prompt = "Name")]

        public string Name { get; set; }
        public long EntityId { get; set; }
        public AttachmentEntityType EntityType { get; set; }
        public DateTime CreatedOn { get; set; }
        //public bool IsDeleted { get; set; }
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

    public class AttachmentSearchVM : BaseSearchModel
    {
        public string? Name { get; set; }
        public AttachmentEntityType? EntityType { get; set; }
        public long? SupplierId { get; set; }
        public long? EntityId { get; set; }
    }
}
