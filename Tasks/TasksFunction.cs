using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using Azure_Function.Tasks.DTO.Database;
using Azure_Function.Tasks.DTO.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Azure_Function.Tasks
{
    public class TasksFunction(ILogger<TasksFunction> logger, ITaskService taskService, QueueClient queueNotificationClient)
    {
        private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);
        private readonly ILogger<TasksFunction> _logger = logger;
        private readonly ITaskService _taskService = taskService;

        private readonly QueueClient _queueNotificationClient = queueNotificationClient;
        [Function("CreateTaskFunction")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "tasks")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger CreateTaskFunction function processed a request.");

            var task = await JsonSerializer.DeserializeAsync<CreateTaskRequest>(
            req.Body, _jsonOptions);


            if (task is null)
            {
                _logger.LogError("Failed to deserialize request body into CreateTaskRequest.");
                return new BadRequestResult();
            }

            var createdTask = _taskService.CreateTask(new CreateTask(
                title: task.Title,
                description: task.Description,
                dueDate: task.DueDate,
                userUpdated: "system"
            ));
            var response = new
            {
                status = "success",
                code = 200,
                data = createdTask
            };

            _logger.LogInformation("Task created with ID: {task}", JsonSerializer.Serialize(createdTask, _jsonOptions));
            var result = await _queueNotificationClient.SendMessageAsync(JsonSerializer.Serialize(createdTask, _jsonOptions));

            _logger.LogInformation("Enqueued notification for created task with ID: {taskId}", result.ToString());
            return new OkObjectResult(response);
        }
    }
}