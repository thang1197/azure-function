using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureFunction.Utility;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AzureFunction.DurableFunctions.ActivityFunction
{
    public class TaskActivities(ILogger<TaskActivities> logger)
    {
        private readonly ILogger<TaskActivities> _logger = logger;

        [Function(nameof(SendEmail))]
        public async Task SendEmail([ActivityTrigger] string message)
        {
            var randomTime = RandomHelper.GetRandomNumber(1, 3) * 1000;
            await Task.Delay(randomTime);
            _logger.LogInformation($"Sending email with message: {message}");
        }
    }
}