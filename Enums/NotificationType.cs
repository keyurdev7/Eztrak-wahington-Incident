using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enums
{
    public enum NotificationType
    {
        Email = 1,
        Sms = 2,
        Push = 3
    }

    public enum NotificationEntityType
    {
        WorkOrderCreated = 1,
        WorkOrderUpdated = 2,
        ResetPassword = 3,
        TicketResponseSubmitted = 4
    }
}
