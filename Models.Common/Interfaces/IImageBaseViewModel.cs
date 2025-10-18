using Microsoft.AspNetCore.Http;

namespace Models.Common.Interfaces
{
    public interface IImageBaseViewModel
    {
        string BaseFolder { get; }
        public IFormFile Image { get; set; }
        public string ImageUrl { get; set; }
        public string DisplayImageUrl
        {
            get;
        }
    }
}
