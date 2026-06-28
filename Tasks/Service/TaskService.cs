using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureFunction.Tasks.DTO;

namespace AzureFunction.Tasks.Service
{
    public class TaskService : ITaskService
    {
        public async Task<Entity.Task> CreateTask(CreateTaskRequest request)
        {
            throw new NotImplementedException();
        }
    }
}