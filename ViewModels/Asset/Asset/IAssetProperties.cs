using System;
using Microsoft.AspNetCore.Http;
using ViewModels.Shared;

namespace ViewModels
{
    public interface IAssetProperties : IAttachmentList, IAssetAssociationList
    {
        List<AttachmentVM> ImagesList { get; set; }
    }
    public interface IAttachmentList
    {
        List<IFormFile> Images { get; set; }
    }
    public interface IAssetAssociationList
    {
        List<AssetAssociationDetailViewModel> AssetAssociations { get; set; }
    }
}

