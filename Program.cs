using Azure.Storage.Queues;
using Azure_Function.Services;
using Azure_Function.Users;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Azure_Function.Tasks;
using Azure_Function.Configuration.Database;
using Microsoft.EntityFrameworkCore;
using Azure_Function.Projects.Repository;
using Azure_Function.Projects;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

// Configure Database Context
builder.Services.AddDbContext<AppDbContext>(opts =>
{
    var connectionString = Environment.GetEnvironmentVariable("TargetEnvironment") == "Development"
        ? Environment.GetEnvironmentVariable("LocalSqlConnectionString")
        : Environment.GetEnvironmentVariable("SqlConnectionString");

    opts.UseNpgsql(connectionString);
});

// Register Repositories and Services
// Repositories
builder.Services.AddScoped<IProjectsRepository, ProjectsRepository>();

// Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IProjectsService, ProjectsService>();

builder.Services.AddSingleton(x =>
{
    var connectionString = Environment.GetEnvironmentVariable("TargetEnvironment") == "Development" ?
        Environment.GetEnvironmentVariable("AzureWebJobsStorageDev") :
        Environment.GetEnvironmentVariable("AzureWebJobsStorage");

    var queueName = "notification-queue";

    var queueOptions = new QueueClientOptions
    {
        MessageEncoding = QueueMessageEncoding.Base64
    };
    var client = new QueueClient(connectionString, queueName);


    return client;
});

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

builder.Build().Run();
