using System;
using Centangle.Common.ResponseHelpers.Models;
using Enums;
using Models;
using Models.Common.Interfaces;
using Repositories.Interfaces;
using ViewModels;
using ViewModels.Shared;
using ViewModels.WorkOrderTasks;

namespace Repositories.Common
{
    public interface IWorkOrderService<CreateViewModel, UpdateViewModel, DetailViewModel> : IBaseCrud<CreateViewModel, UpdateViewModel, DetailViewModel>
        where DetailViewModel : class, IBaseCrudViewModel, new()
        where CreateViewModel : class, IBaseCrudViewModel, new()
        where UpdateViewModel : class, IBaseCrudViewModel, IIdentitifier, new()
    {
        Task<List<AttachmentVM>> GetWorkOrderAttachments(long id);
        Task<string> GetAttachmentUrl(long id);
        Task<IRepositoryResponse> GetNotesByWorkOrderId(long id);
        Task<IRepositoryResponse> SaveNotes(WorkOrderNotesViewModel model);
        Task<bool> CreateTasks(WorkOrderTasksModifyViewModel model);
        Task<WorkOrderTasksIndexViewModel> GetWorkOrderTasks(long id);
        Task<long> SetWorkOrderTaskStatus(long id, WOTaskStatusCatalog status);
        Task<IRepositoryResponse> UploadImages(WorkOrderAddImageViewModel model);
        Task<IRepositoryResponse> UpdateStatus(WorkOrderModifyStatusViewModel model);
    }
}

