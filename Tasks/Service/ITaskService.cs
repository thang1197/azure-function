using System;
using System.Collections.Generic;
using System.Linq;
using AzureFunction.Tasks.DTO;

namespace AzureFunction.Tasks.Service
{
    public interface ITaskService
    {
        public Task<Entity.Task> CreateTask(CreateTaskRequest request);

        public Task<Entity.Task> UpdateTaskById(int id, UpdateTaskRequest request);
    }
}