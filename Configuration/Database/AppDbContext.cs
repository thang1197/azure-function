using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using AzureFunction.Notifications.Entity;
using AzureFunction.Storage.Entity;
namespace AzureFunction.Configuration.Database
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
        {
        }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<FileStorage> FileStorages { get; set; }
        // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        // {
        //     // Replace with your actual PostgreSQL connection details
        //     string connectionString = "Host=172.24.64.114;Port=5432;Database=task_management;Username=postgres;Password=123456;SearchPath=public";

        //     optionsBuilder.UseNpgsql(connectionString);
        // }
    }
}