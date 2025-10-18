using Helpers.Extensions;
using Helpers.Image;
using ViewModels.Shared;

namespace ViewModels
{
    public class MUTCDDetailViewModel : BaseCrudViewModel
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string? ImageUrl { get; set; }
        public string FormattedImageUrl
        {
            get
            {
                return string.IsNullOrEmpty(ImageUrl) ? ImageHelper.NoImagePath : ImageUrl;
            }
        }
        public DateTime CreatedOn { get; set; }
    }
}
