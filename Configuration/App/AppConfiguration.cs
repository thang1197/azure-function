using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureFunction.Configuration.App
{
    public class AzureStorageSettings
    {
        // It's a good practice to define a constant for the section name
        public const string SectionName = "AzureStorage";

        public string ContainerName { get; set; } = string.Empty;
        public string BlobServiceUri { get; set; } = string.Empty;
    }

    public class AzureAdSettings
    {
        public const string SectionName = "AzureAd";

        public string Instance { get; set; } = string.Empty;
        public string TenantId { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;

        public string ClientSecret { get; set; } = string.Empty;
    }

    public class BlobStorageSettings
    {
        public const string SectionName = "BlobStorage";

        public string ContainerName { get; set; } = string.Empty;
        public string BlobServiceUri { get; set; } = string.Empty;

        public int SasDuration { get; set; }
    }

    public class JwtSettings
    {
        public const string SectionName = "JwtSettings";
        public string Secret { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int ExpiryInMinutes { get; set; }
    }

    public class ApiManagementSettings
    {
        public const string SectionName = "ApiManagement";
        public string SubscriptionId { get; set; } = string.Empty;
        public string ResourceGroupName { get; set; } = string.Empty;
        public string ServiceName { get; set; } = string.Empty;
    }
}