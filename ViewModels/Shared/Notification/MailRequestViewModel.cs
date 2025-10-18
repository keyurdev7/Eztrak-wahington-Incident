using Enums;

namespace ViewModels.Shared.Notification
{
    public class MailRequestViewModel
    {
        public MailRequestViewModel(string sendTo, string subject, string body, long entityId, NotificationEntityType entityType, NotificationType type)
        {
            SendTo = sendTo;
            Subject = subject;
            Body = body;
            EntityId = entityId;
            Type = type;
            EntityType = entityType;
        }
        public string SendTo { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public long EntityId { get; set; }
        public NotificationEntityType EntityType { get; set; }
        public NotificationType Type { get; set; }
    }
}
