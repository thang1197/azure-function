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
using AzureFunction.Utility;
using AzureFunction.Tasks.Service;
using AzureFunction.Middleware;
using Azure.Security.KeyVault.Secrets;
using System.Text.Json.Nodes;
using Microsoft.ApplicationInsights.Extensibility;



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

// 2. Resolve credentials upfront or read them from config
var azureAdSettings = builder.Configuration
    .GetSection(AzureAdSettings.SectionName)
    .Get<AzureAdSettings>() ?? throw new Exception("Azure AD settings not found");

var clientSecretCredential = new ClientSecretCredential(
    azureAdSettings.TenantId,
    azureAdSettings.ClientId,
    azureAdSettings.ClientSecret
);

builder.Services.AddSingleton(clientSecretCredential);

var keyVaultUrl = Environment.GetEnvironmentVariable("KeyVault:Url") ?? "";

if (string.IsNullOrEmpty(keyVaultUrl))
{
    throw new Exception("KeyVault:Url not found");
}
var keyVaultClient = new SecretClient(new Uri(keyVaultUrl), clientSecretCredential);
builder.Services.AddSingleton(keyVaultClient);

// Key Vault
var KeyVaulValues = new JsonObject
{
   {ApplicationInsightSettings.SectionName, ApplicationInsightSettings.KeyVaultName}
};
for (int i = 0; i < KeyVaulValues.Count; i++)
{
    var sectionName = KeyVaulValues.ElementAt(i).Key;
    var secretKey = KeyVaulValues.ElementAt(i).Value?.ToString() ?? string.Empty;
    Console.WriteLine($"Loading secret for {sectionName} from Key Vault with key: {secretKey}");

    var secret = keyVaultClient.GetSecret(secretKey).Value;
    Console.WriteLine($"Loaded secret for {sectionName} from Key Vault: {secret.Value}");
    KeyVaulValues[sectionName] = secret.Value;
}


// Load environment variables
builder.Services.Configure<BlobStorageSettings>(builder.Configuration.GetSection(BlobStorageSettings.SectionName));
builder.Services.Configure<AzureAdSettings>(builder.Configuration.GetSection(AzureAdSettings.SectionName));
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.SectionName));
builder.Services.Configure<ApiManagementSettings>(builder.Configuration.GetSection(ApiManagementSettings.SectionName));
builder.Services.Configure<ApplicationInsightSettings>(x =>
{
    var connectionString = KeyVaulValues[ApplicationInsightSettings.SectionName] ?? throw new Exception("ApplicationInsightSettings not found");  
    x.ConnectionString = connectionString.ToString();
});

// Register Repositories and Services
// Repositories
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();

// Services
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IBlobStorageService, BlobStorageService>();
builder.Services.AddScoped<IMailService, MailService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ITaskService, TaskService>();

builder.Services.AddSingleton(x =>
{
    if (Environment.GetEnvironmentVariable("TargetEnvironment") == "Development")
    {
        var blobServiceClient = new BlobServiceClient(
            Environment.GetEnvironmentVariable("AzureWebJobsStorageDev")
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

// Http Client
builder.Services.AddSingleton(x =>
{
    var azureAdSettings = x.GetRequiredService<IOptions<AzureAdSettings>>().Value;
    var apiManagementSettings = x.GetRequiredService<IOptions<ApiManagementSettings>>().Value;
    var clientCredential = x.GetRequiredService<ClientSecretCredential>();

    return new HttpHelper(apiManagementSettings, azureAdSettings, clientCredential);
});

builder.Services
    .AddApplicationInsightsTelemetryWorkerService(options =>
    {
        var connectionString = KeyVaulValues[ApplicationInsightSettings.SectionName] ?? throw new Exception("ApplicationInsightSettings not found");
        Console.WriteLine($"Loaded secret for {ApplicationInsightSettings.SectionName} from Key Vault: {connectionString}");
        options.ConnectionString = connectionString.ToString();
    })
    .ConfigureFunctionsApplicationInsights();


builder.UseMiddleware<JwtAuthMiddleware>();
var app = builder.Build();

app.Run();
