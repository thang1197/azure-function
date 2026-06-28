using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureFunction.Notifications.DTO;
using AzureFunction.Notifications.Entity;
using AzureFunction.Notifications.Repository;

namespace AzureFunction.Notifications.Service
{
    public class NotificationService(INotificationRepository notificationRepository) : INotificationService
    {
        private readonly INotificationRepository _notificationRepository = notificationRepository;
        public Notification SendNotification(CreateNotification dto)
        {
           try
            {
                var notification = _notificationRepository.CreateNotification(dto);
                return notification;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error sending notification: {ex.Message}");
            }
        }
    }
}