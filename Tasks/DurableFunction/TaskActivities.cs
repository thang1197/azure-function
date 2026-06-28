using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureFunction.Tasks.DTO;
using AzureFunction.Tasks.Service;
using Microsoft.Azure.Functions.Worker;

namespace AzureFunction.Tasks.DurableFunction
{
    public class TaskActivities(ITaskService taskService)
    {
        private readonly ITaskService _taskService = taskService;
        [Function(nameof(CreateTask))]
        public async Task<Entity.Task> CreateTask([ActivityTrigger] CreateTaskRequest task)
        {
            var createdTask = await _taskService.CreateTask(task);
            return createdTask;
        }
    }
}