using Enums;
using Models.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Notification : BaseDBModel
    {
        public long EntityId { get; set; }
        public NotificationEntityType EntityType { get; set; }
        public string Message { get; set; }
        public string Subject { get; set; }
        public string SendTo { get; set; }
        public long ResendCount { get; set; }
        public bool IsSent { get; set; }
        public NotificationType Type { get; set; }
    }
}
