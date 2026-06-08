using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure_Function.Projects.DTO.Database;
using Azure_Function.Projects.Repository;

namespace Azure_Function.Projects
{
    public class ProjectsService(IProjectsRepository repository) : IProjectsService
    {
        private readonly IProjectsRepository _repository = repository;
        public Project CreateProject(CreateProject dto)
        {
            try
            {
                return _repository.CreateProject(dto);
            }
            catch (Exception e)
            {
                throw new Exception("Error creating project", e);
            }
        }

        public List<Project> GetAllProjects()
        {
            try
            {
                return _repository.GetAllProjects();
            }
            catch (Exception e)
            {
                throw new Exception("Error fetching projects", e);
            }
        }

        public Project GetProjectById(int id)
        {
            throw new NotImplementedException();
        }
    }
}