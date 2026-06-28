using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureFunction.Configuration.Database;
using AzureFunction.Notifications.DTO;
using AzureFunction.Notifications.Entity;

namespace AzureFunction.Notifications.Repository
{
    public class NotificationRepository(AppDbContext context) : INotificationRepository
    {
        private readonly AppDbContext _context = context;
        public Notification CreateNotification(CreateNotification dto)
        {
            try
            {
                var notification = new Notification
                {
                    Title = dto.Title,
                    Description = dto.Description,
                    Sender = dto.Sender,
                    Receiver = dto.Receiver,
                    Type = dto.Type,
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var result = _context.Notifications.Add(notification);
                _context.SaveChanges();

                return result.Entity;
            }
            catch (Exception ex)
            {

                throw new Exception("Error creating notification", ex);
            }
        }

        public List<Notification> GetAllNotifications()
        {
            try
            {
                var notifications = _context.Notifications.ToList();
                return notifications;
            }
            catch (System.Exception ex)
            {
                
                throw new Exception("Error retrieving notifications: " + ex.Message);
            }
        }

        public Notification GetNotificationById(int id)
        {
            throw new NotImplementedException();
        }

        public List<Notification> GetNotificationsByReceiver(string receiver)
        {
            try
            {
                return _context.Notifications.Where(n => n.Receiver == receiver).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving notifications by receiver", ex);
            }
        }
    }
}