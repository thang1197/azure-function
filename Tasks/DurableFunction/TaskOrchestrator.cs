using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureFunction.Tasks.DTO;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;

namespace AzureFunction.Tasks.DurableFunction
{
    public class TaskOrchestrator
    {
        [Function(nameof(BatchInsertTasks))]
        public async Task<List<Entity.Task>> BatchInsertTasks([OrchestrationTrigger] TaskOrchestrationContext context)
        {
            var tasks = context.GetInput<List<CreateTaskRequest>>() ?? throw new ArgumentException("Tasks cannot be null");
            
            var listTasks = new List<Task<Entity.Task>>();

            foreach (var task in tasks)
            {
                listTasks.Add(context.CallActivityAsync<Entity.Task>(nameof(TaskActivities.CreateTask), task));
            }

            var result = await Task.WhenAll(listTasks);
            return result.ToList();
        }
    }
}