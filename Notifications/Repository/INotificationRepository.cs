using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureFunction.Notifications.DTO;
using AzureFunction.Notifications.Entity;

namespace AzureFunction.Notifications.Repository
{
    public interface INotificationRepository
    {
        public Notification CreateNotification(CreateNotification dto);
        public Notification GetNotificationById(int id);
        public List<Notification> GetAllNotifications();
        public List<Notification> GetNotificationsByReceiver(string receiver);
    }
}