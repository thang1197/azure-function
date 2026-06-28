using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.EventGrid;
using Azure.Storage.Queues.Models;
using AzureFunction.Notifications.DTO;
using AzureFunction.Notifications.Service;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AzureFunction.Notifications.EventGrid
{
    public class NotificationTrigger(INotificationService notificationService, ILogger<NotificationTrigger> logger)
    {
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
        };
        private readonly ILogger<NotificationTrigger> _logger = logger;
        private readonly INotificationService _notificationService = notificationService;
        [Function(nameof(NotificationTrigger))]
        public void Run([EventGridTrigger] EventGridEvent eventGridEvent)
        {
            _logger.LogInformation("C# EventGrid trigger function processed: {eventData}", eventGridEvent.Data);
            try
            {
                var notificationData = JsonSerializer.Deserialize<CreateNotification>(eventGridEvent.Data.ToString(), _jsonOptions);
                if (notificationData == null)
                {
                    _logger.LogError("Failed to deserialize event data to CreateNotification.");
                    return;
                }

                var validationResults = Utility.HttpHelper.ValidateRequestBody(notificationData);
                if (validationResults != null && validationResults.Any())
                {
                    _logger.LogError("Validation failed for CreateNotification: {validationErrors}", string.Join(", ", validationResults));
                    return;
                }

                var notification = _notificationService.SendNotification(notificationData);
                _logger.LogInformation("Notification sent successfully: {notificationId}", notification.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing EventGrid event: {errorMessage}", ex.Message);
            }
        }
    }
}