using Models.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace ViewModels
{
    public class AssetAddImageViewModel : IIdentitifier, IAttachmentList
    {
        public long Id { get; set; }
        public List<IFormFile> Images { get; set; } = new();
    }
}
