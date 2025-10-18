using Helpers.File;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ViewModels.Authentication
{
    public class UserUpdateVM : IFileModel
    {
        public long Id { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Required]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^[0-9]{7,15}$", ErrorMessage = "Phone no. must be a valid number")] 
        public string PhoneNumber { get; set; }
        public string ImageUrl { get; set; }
        public IFormFile File { get; set; }

        public string GetBaseFolder()
        {
            var ext = Path.GetExtension(File.FileName);
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
