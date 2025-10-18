using Microsoft.Extensions.Logging;
using NotificationWorkerService.Interface;

namespace NotificationWorkerService.Repository
{
    public class PushNotificationService : IPushNotification
    {
        private readonly ILogger<PushNotificationService> _logger;

        public PushNotificationService(ILogger<PushNotificationService> logger)
        {
            _logger = logger;
        }
        //public async Task<bool> SendPushNotification(Notification notification, string deviceId)
        //{
        //    try
        //    {

        //        var notificationBody = JsonConvert.DeserializeObject<LogPushNotificationViewModel>(notification.Message);

        //        var expoSDKClient = new PushApiClient();
        //        var pushTicketReq = new PushTicketRequest()
        //        {
        //            PushBadgeCount = 0,
        //            PushData = notificationBody,
        //            PushTitle = notificationBody.Title,
        //            PushBody = notificationBody.Message
        //        };

        //        pushTicketReq.PushTo = new List<string>() { deviceId };
        //        //pushTicketReq.PushTo = new List<string>() { "PRMSxIGfT1w_sWA9xXqK_9" };

        //        var result = await expoSDKClient.PushSendAsync(pushTicketReq);
        //        _logger.LogInformation($"Entity Id: {notification.EntityId}");
        //        if (result == null || result?.PushTicketErrors?.Count() > 0)
        //            return false;
        //        foreach (var stat in result?.PushTicketStatuses)
        //        {
        //            if (stat.TicketStatus == "error")
        //                return false;
        //        }
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}
    }

}

