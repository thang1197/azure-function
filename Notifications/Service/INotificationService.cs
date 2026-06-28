using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureFunction.Notifications.DTO;
using AzureFunction.Notifications.Entity;

namespace AzureFunction.Notifications.Service
{
    public interface INotificationService
    {
        public Notification SendNotification(CreateNotification dto);
    }
}