using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure_Function.Projects.DTO.Database;

namespace Azure_Function.Projects
{
    public interface IProjectsService
    {
        public Project CreateProject(CreateProject dto);
        public Project GetProjectById(int id);
        public List<Project> GetAllProjects();
    }
}