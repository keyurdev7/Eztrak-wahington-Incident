using Enums;
using Pagination;

namespace ViewModels.Shared.Notification
{
    public class NotificationSearchViewModel : BaseSearchModel
    {
        public long Id { get; set; }
        public NotificationType Type { get; set; }
    }
}
