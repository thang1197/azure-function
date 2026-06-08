using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure_Function.Projects;
using Microsoft.EntityFrameworkCore;
namespace Azure_Function.Configuration.Database
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
        {
        }
        public DbSet<Project> Projects { get; set; }
        // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        // {
        //     // Replace with your actual PostgreSQL connection details
        //     string connectionString = "Host=172.24.64.114;Port=5432;Database=task_management;Username=postgres;Password=123456;SearchPath=public";

        //     optionsBuilder.UseNpgsql(connectionString);
        // }
    }
}