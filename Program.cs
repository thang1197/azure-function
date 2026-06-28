using Azure.Storage.Queues;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AzureFunction.Configuration.Database;
using Microsoft.EntityFrameworkCore;
using Azure.Identity;
using Azure.Storage.Blobs;
using AzureFunction.Storage.Service;
using AzureFunction.Mail.Serivce;
using AzureFunction.Notifications.Repository;
using AzureFunction.Notifications.Service;
using Microsoft.Extensions.Configuration;
using AzureFunction.Configuration.App;
using Microsoft.Extensions.Options;
using AzureFunction.Authentication.Service;
using Azure.ResourceManager;
using AzureFunction.Products.Service;


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

// Load environment variables
builder.Services.Configure<BlobStorageSettings>(builder.Configuration.GetSection(BlobStorageSettings.SectionName));
builder.Services.Configure<AzureAdSettings>(builder.Configuration.GetSection(AzureAdSettings.SectionName));
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.SectionName));
builder.Services.Configure<ApiManagementSettings>(builder.Configuration.GetSection(ApiManagementSettings.SectionName));

// Register Repositories and Services
// Repositories
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();

// Services
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IBlobStorageService, BlobStorageService>();
builder.Services.AddScoped<IMailService, MailService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddSingleton(x =>
{
    if (Environment.GetEnvironmentVariable("TargetEnvironment") == "Development")
    {
        var blobServiceClient = new BlobServiceClient(
            new Uri(Environment.GetEnvironmentVariable("AzureWebJobsStorageDev")!)
        );

        return blobServiceClient;
    }
    else
    {
        var blogStorageSettings = x.GetRequiredService<IOptions<BlobStorageSettings>>().Value;
        var azureAdSettings = x.GetRequiredService<IOptions<AzureAdSettings>>().Value;

        var credentials = new ClientSecretCredential(
            azureAdSettings.TenantId,
            azureAdSettings.ClientId,
            azureAdSettings.ClientSecret
        );

        var blobServiceClient = new BlobServiceClient(
            new Uri(blogStorageSettings.BlobServiceUri),
            credentials
        );

        return blobServiceClient;
    }
});

builder.Services.AddSingleton(x =>
{
    var credentials = new ClientSecretCredential(
        Environment.GetEnvironmentVariable("AzureTenantId"),
        Environment.GetEnvironmentVariable("AzureClientId"),
        Environment.GetEnvironmentVariable("AzureClientSecret")
    );

    var queueName = "notification-queue";
    var uri = new Uri("https://thanglhstorage.queue.core.windows.net/" + queueName);

    var queueOptions = new QueueClientOptions
    {
        MessageEncoding = QueueMessageEncoding.Base64
    };
    var client = new QueueClient(uri, credentials);


    return client;
});

// ARM Client
builder.Services.AddSingleton(x =>
{
    var azureAdSettings = x.GetRequiredService<IOptions<AzureAdSettings>>().Value;

    var credentials = new ClientSecretCredential(
        azureAdSettings.TenantId,
        azureAdSettings.ClientId,
        azureAdSettings.ClientSecret
    );

    return new ArmClient(credentials);
});

// Client Secret Credential
builder.Services.AddSingleton(x =>
{
    var azureAdSettings = x.GetRequiredService<IOptions<AzureAdSettings>>().Value;

    return new ClientSecretCredential(
        azureAdSettings.TenantId,
        azureAdSettings.ClientId,
        azureAdSettings.ClientSecret
    );
});

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

builder.Build().Run();
