using Enums;
using ViewModels.Shared.Notification;

namespace Repositories.Shared.NotificationServices.Interface
{
    public interface INotificationService
    {
        Task<bool> AddNotificationAsync(NotificationViewModel model);
        Task<bool> MappNotification(MailRequestViewModel mailRequest);
    }
}
