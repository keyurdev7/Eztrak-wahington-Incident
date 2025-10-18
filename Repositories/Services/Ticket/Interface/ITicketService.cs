using System;
using Models.Common.Interfaces;
using Repositories.Interfaces;
using ViewModels;
using ViewModels.Shared;
using ViewModels.Ticket;

namespace Repositories.Common
{
    public interface ITicketService<CreateViewModel, UpdateViewModel, DetailViewModel> : IBaseCrud<CreateViewModel, UpdateViewModel, DetailViewModel>
        where DetailViewModel : class, IBaseCrudViewModel, new()
        where CreateViewModel : class, IBaseCrudViewModel, new()
        where UpdateViewModel : class, IBaseCrudViewModel, IIdentitifier, new()
    {
        Task<List<TicketHistoryDetailViewModel>> GetTicketHistory(int id);
        Task<bool> UpdateTicket(TicketStatusUpdateViewModel model);
        Task<List<AttachmentVM>> GetTicketAttachments(long id);
        Task<string> GetAttachmentUrl(long id);
    }
}

