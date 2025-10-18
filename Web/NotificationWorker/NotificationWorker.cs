using DataLibrary;
using Enums;
using Interfaces;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NotificationWorkerService.Interface;
using ViewModels.Shared.Notification;

namespace NotificationWorkerService;

public class NotificationWorker : BackgroundService
{
    private readonly ILogger<NotificationWorker> _logger;
    private readonly ApplicationDbContext _db;
    private readonly IConfiguration _configuration;
    private readonly IEmail _emailService;
    private readonly ISms _smsService;
    private readonly IPushNotification _pushNotification;

    public NotificationWorker(IServiceScopeFactory factory)
    {
        _logger = factory.CreateScope().ServiceProvider.GetRequiredService<ILogger<NotificationWorker>>();
        _configuration = factory.CreateScope().ServiceProvider.GetRequiredService<IConfiguration>();
        _emailService = factory.CreateScope().ServiceProvider.GetRequiredService<IEmail>();
        _smsService = factory.CreateScope().ServiceProvider.GetRequiredService<ISms>();
        _pushNotification = factory.CreateScope().ServiceProvider.GetRequiredService<IPushNotification>();
        _db = factory.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Notification Worker running at: {time}", DateTimeOffset.Now);

            await SendNotifications();
            await Task.Delay(20000, stoppingToken);
        }
    }

    public async Task<bool> SendNotifications()
    {
        try
        {
            var currentDate = DateTime.UtcNow;
            var notifications = await _db.Notifications.Where(x => x.ResendCount < 5 && x.SendTo != null && x.CreatedOn.Date <= currentDate.Date && x.IsSent == false).Skip(0).Take(5).ToListAsync();
            var emails = notifications.Where(x => x.Type == NotificationType.Email).ToList();
            var smss = notifications.Where(x => x.Type == NotificationType.Sms).ToList();
            var pushNotifications = notifications.Where(x => x.Type == NotificationType.Push).ToList();
            var sendToEmails = notifications.Where(x => !string.IsNullOrEmpty(x.SendTo)).Select(x => x.SendTo).Distinct().ToList();
           // var users = await _db.Users.Where(x => sendToIds.Contains(x.Id)).ToListAsync();
            var appEmail = _configuration["AppEmail"];
            foreach (var email in emails)
            {
              //  var sendToEmail = users.Where(x => x.Id.ToString() == email.SendTo).Select(x => x.Email).FirstOrDefault();
               // var baseModel = JsonConvert.DeserializeObject<EmailBaseModel>(email.Message);
                var emailResult = await _emailService.SendEmail(email.SendTo ?? "", appEmail ?? "", email.Subject ?? "", email.Message);
                if (emailResult)
                {
                    email.IsSent = true;
                }
                else
                {
                    email.IsSent = false;
                    email.ResendCount += 1;
                }
                await _db.SaveChangesAsync();
            }
            foreach (var sms in smss)
            {
                var smsResult = await _smsService.SendSms(sms.SendTo, sms.Message);
                if (smsResult)
                    sms.IsSent = true;
                else
                {
                    sms.IsSent = false;
                    sms.ResendCount += 1;
                }
                await _db.SaveChangesAsync();
            }
            //if (pushNotifications.Count > 0)
            //{
            //    var userIds = pushNotifications.Select(x => long.Parse(x.SendTo)).ToList();
            //    var deviceIds = await _db.Users.Where(x => userIds.Contains(x.Id) && x.IsDeleted == false).Select(x => new { DeviceId = x.DeviceId, UserId = x.Id.ToString() }).ToListAsync();

            //    foreach (var notification in pushNotifications)
            //    {
            //        var deviceId = deviceIds.Where(x => x.UserId == notification.SendTo).Select(x => x.DeviceId).FirstOrDefault();
            //        if (deviceId != null)
            //        {
            //            var pushNotificationResult = await _pushNotification.SendPushNotification(notification, deviceId);
            //            if (pushNotificationResult)
            //                notification.IsSent = true;
            //            else
            //            {
            //                notification.IsSent = false;
            //                notification.ResendCount += 1;
            //            }
            //        }
            //        else
            //        {
            //            notification.ResendCount += 1;
            //        }
            //        await _db.SaveChangesAsync();
            //    }
            //}

            return true;
        }
        catch (Exception ex)
        {
        }
        _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
        return false;

    }
}

