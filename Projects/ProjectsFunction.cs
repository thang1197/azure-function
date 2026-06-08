using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using Azure.Storage.Queues;
using Azure_Function.Projects.DTO.Http;
using Azure_Function.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace Azure_Function.Projects
{
    public class ProjectsFunction(IProjectsService projectsService, QueueClient queueClient)
    {
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        private readonly IProjectsService _projectsService = projectsService;
        private readonly QueueClient _queueClient = queueClient;

        [Function("CreateProjectsFunction")]
        public async Task<IActionResult> CreateProject(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "projects")] HttpRequest req)
        {
            var project = await JsonSerializer.DeserializeAsync<CreateProjectsRequest>(req.Body, options: _jsonOptions);

            if (project is null)
            {
                return new BadRequestResult();
            }

            Project createdProject = _projectsService.CreateProject(new DTO.Database.CreateProject
            {
                Name = project.Name,
                Description = project.Description,
                Owner = project.Owner,
                Status = project.Status,
                StartDate = project.StartDate,
                EndDate = project.EndDate
            });

            var response = HttpHelper.GenerateResponse(createdProject, statusCode: StatusCodes.Status201Created);

            // Send notification to queue
            var notification = new Notification.DTO.CreateNotification
            {
                Title = $"New Project Created: {createdProject.Name}",
                Description = $"Project '{createdProject.Name}' has been created by {createdProject.Owner}.",
                Sender = "ProjectsFunction",
                Receiver = "NotificationService",
                Type = "ProjectCreation"
            };
            await _queueClient.SendMessageAsync(JsonSerializer.Serialize(notification, _jsonOptions));
            
            return response;
        }

        [Function("GetProjectsFunction")]
        public async Task<IActionResult> GetProjects([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "projects")] HttpRequest req)
        {
            var projects = _projectsService.GetAllProjects();
            var response = HttpHelper.GenerateResponse(projects);
            return response;
        }
    }
}