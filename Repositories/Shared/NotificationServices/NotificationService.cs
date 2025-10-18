using AutoMapper;
using DataLibrary;
using Enums;
using Microsoft.Extensions.Logging;
using Models;
using Repositories.Shared.NotificationServices.Interface;
using ViewModels.Shared.Notification;

namespace Repositories.Shared.NotificationServices
{
    public class NotificationService : INotificationService
    {
        private readonly ILogger<NotificationService> _logger;
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public NotificationService(
            ILogger<NotificationService> logger,
            ApplicationDbContext db,
            IMapper mapper
            )
        {
            _logger = logger;
            _db = db;
            _mapper = mapper;
        }


        public async Task<bool> MappNotification(MailRequestViewModel mailRequest)
        {
            try
            {
                NotificationViewModel viewModel = new()
                {
                    EntityId = mailRequest.EntityId,
                    Message = mailRequest.Body,
                    SendTo = mailRequest.SendTo,
                    EntityType = mailRequest.EntityType,
                    Type = mailRequest.Type,
                    Subject = mailRequest.Subject
                };

                if (await AddNotificationAsync(viewModel))
                {
                    return true;
                }
                return false;

                //message = $"Reset Password for SMS {mailRequest.SendTo}";
                //await _notification.AddNotificationAsync(mailRequest.EntityId, NotificationEntityType.All, message, mailRequest.Subject, mailRequest.ToEmail, NotificationType.Sms, null);


                //await _notification.AddNotificationAsync(appId.ToString(), NotificationEntityType.Application, message, "Lab App Sign Up", user.MobileNo, NotificationType.Sms, loginUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError($"IdentityService SendMessage method threw an exception, Message: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> AddNotificationAsync(NotificationViewModel model)
        {
            try
            {
                var notificationModel = _mapper.Map<Notification>(model);
                _db.Add(notificationModel);
                await _db.SaveChangesAsync();
                _logger.LogInformation("Notification has been sent!");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"AddNotification threw an exception, Message: {ex.Message}");
            }
            return false;
        }
    }
}
