using Helpers.Attributes;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Helpers.File
{
    public interface IFileModel
    {
        [DataType(DataType.Upload)]
        [MaxFileSize(5 * 1024 * 1024)]
        [AllowedExtensions(new string[] { ".jpg", ".png", ".jpeg" })]
        IFormFile File { get; set; }

        string GetBaseFolder();
    }
}
