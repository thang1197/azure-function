using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure_Function.Projects;
using Azure_Function.Projects.DTO.Database;

namespace Azure_Function.Projects.Repository
{
    public interface IProjectsRepository
    {
        public Project CreateProject(CreateProject dto);
        public List<Project> GetAllProjects();
    }
}