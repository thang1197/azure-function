using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure_Function.Tasks.DTO;
using Azure_Function.Tasks.DTO.Database;

namespace Azure_Function.Tasks
{
    public interface ITaskService
    {
        public TaskEntity CreateTask(CreateTask dto);
        public TaskEntity GetTaskById(int id);
        public List<TaskEntity> GetAllTasks();
    }
}