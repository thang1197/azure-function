using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure_Function.Tasks.DTO;
using Azure_Function.Tasks.DTO.Database;

namespace Azure_Function.Tasks
{
    public class TaskService : ITaskService
    {
        public TaskEntity CreateTask(CreateTask dto)
        {
            return new TaskEntity(1, dto.Title, dto.Description, dto.DueDate);
        }

        public List<TaskEntity> GetAllTasks()
        {
            throw new NotImplementedException();
        }

        public TaskEntity GetTaskById(int id)
        {
            throw new NotImplementedException();
        }
    }
}