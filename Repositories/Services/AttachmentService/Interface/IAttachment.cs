using Centangle.Common.ResponseHelpers.Models;
using Enums;
using Repositories.Interfaces;
using ViewModels.Shared;

namespace Repositories.Services.AttachmentService.Interface
{
    public interface IAttachment : IBaseSearch
    {
        Task<IRepositoryResponse> CreateMultiple(List<AttachmentVM> model);
        Task<IRepositoryResponse> CreateSingle(AttachmentVM attachment);
        Task<IRepositoryResponse> Update(List<AttachmentVM> model, long entityId, AttachmentEntityType type);
        Task<IRepositoryResponse> Delete(List<long> attachmentIds);
        Task<IRepositoryResponse> DeleteByEntity(long entityId, AttachmentEntityType entityType);
        Task<List<string>> GetUrls(AttachmentEntityType type, long entityId);
    }
}
