using System;
using System.Text.Json;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Azure_Function.QueueFunctions;

public class NotificationQueueFunction
{
    private readonly ILogger<NotificationQueueFunction> _logger;

    public NotificationQueueFunction(ILogger<NotificationQueueFunction> logger)
    {
        _logger = logger;
    }

    // [Function(nameof(NotificationQueueFunction))]
    // public void Run([QueueTrigger("notification-queue", Connection = "AzureWebJobsStorage")] QueueMessage message)
    // {
    //     _logger.LogInformation("C# Queue trigger function processed: {messageText}", message.MessageText);
    // }
}