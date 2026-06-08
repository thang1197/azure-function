using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Azure_Function.Configuration.Database;
using Azure_Function.Projects.DTO.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Azure_Function.Projects.Repository
{
    public class ProjectsRepository(AppDbContext dbContext, ILogger<ProjectsRepository> logger) : IProjectsRepository
    {
        private readonly AppDbContext _dbContext = dbContext;
        private readonly ILogger<ProjectsRepository> _logger = logger;
        public Project CreateProject(CreateProject dto)
        {
            var project = new Project
            {
                Name = dto.Name,
                Description = dto.Description,
                CreatedAt = DateTime.UtcNow,
                Status = dto.Status,
                Owner = dto.Owner,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate
            };

            _logger.LogInformation("Creating project: {@Project}", JsonSerializer.Serialize(project));
            _dbContext.Projects.Add(project);
            _dbContext.SaveChanges();
            return project;
        }

        public List<Project> GetAllProjects()
        {
            return _dbContext.Projects.ToList();
        }
    }
}