
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using AzureFunction.Configuration.App;
using AzureFunction.Storage.DTO;
using AzureFunction.Storage.Entity;
using AzureFunction.Utility;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AzureFunction.Storage.Service
{
    public class BlobStorageService(
        ILogger<BlobStorageService> logger,
        BlobServiceClient blobServiceClient,
        IOptions<BlobStorageSettings> blobStorageSettings
        ) : IBlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient = blobServiceClient;
        private readonly BlobStorageSettings _blobStorageSettings = blobStorageSettings.Value;
        private readonly ILogger<BlobStorageService> _logger = logger;
        public async Task<FileStorage> UploadAsync(string fileName,string containerName, Stream fileContent, IDictionary<string, string>? metaData)
        {
            var formatFileName = Guid.NewGuid() + Path.GetExtension(fileName);
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

            if (!await containerClient.ExistsAsync())
            {
                throw new Exception($"Container '{containerName}' does not exist.");
            }

            var blobClient = containerClient.GetBlobClient(formatFileName);

            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(fileName, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            var fileMetaData = new Dictionary<string, string> {
                { "fileName", fileName },
                { "contentType", contentType },
                { "size", fileContent.Length.ToString() },
                { "folder", containerName }
            };

            if (metaData != null)
            {
                foreach (var item in metaData)
                {
                    fileMetaData.Add(item.Key, item.Value);
                }
            }

            var blobOptions = new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders { ContentType = contentType },
                Metadata = fileMetaData
            };

            var response = await blobClient.UploadAsync(fileContent, blobOptions);
            var fileStorage = new FileStorage
            {
                Name = formatFileName,
                ContentType = contentType,
                Size = fileContent.Length,
                Url = blobClient.Uri.ToString(),
                Version = response.Value.VersionId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                UserCreated = metaData != null ? metaData["userCreated"] : "default",
                UserUpdated = metaData != null ? metaData["userUpdated"] : "default"
            };

            return fileStorage;
        }

        public async Task<bool> DeleteAsync(string blobName, string containerName)
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

                if (!await containerClient.ExistsAsync())
                {
                    throw new Exception($"Container '{containerName}' does not exist.");
                }

                var blobClient = containerClient.GetBlobClient(blobName);
                await blobClient.DeleteIfExistsAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting blob: {ex.Message}");
            }
        }

        public async Task<List<string>> ListFilesAsync(string containerName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

            if (!await containerClient.ExistsAsync())
            {
                throw new Exception($"Container '{containerName}' does not exist.");
            }

            var blobs = containerClient.GetBlobsAsync();
            var blobNames = new List<string>();

            await foreach (var blob in blobs)
            {
                blobNames.Add(blob.Name);
            }

            return blobNames;
        }

        public async Task<BlobStreamDownload> DownloadAsync(string blobName)
        {
            var containerName = _blobStorageSettings.ContainerName;

            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            if (!await containerClient.ExistsAsync())
            {
                throw new Exception($"Container '{containerName}' does not exist.");
            }
            var blobClient = containerClient.GetBlobClient(blobName);

            BlobDownloadStreamingResult downloadResult = await blobClient.DownloadStreamingAsync();

            return new BlobStreamDownload
            {
                Content = downloadResult.Content,
                ContentType = downloadResult.Details.ContentType,
                FileName = blobName
            };
        }

        public async Task<string> GenerateSasUri(string blobName)
        {
            try
            {
                var containerName = _blobStorageSettings.ContainerName;
                var expiresOn = DateTimeOffset.UtcNow.AddSeconds(_blobStorageSettings.SasDuration);

                var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

                if (!await containerClient.ExistsAsync())
                {
                    throw new Exception($"Container '{containerName}' does not exist.");
                }

                var blobClient = containerClient.GetBlobClient(blobName);

                if (await blobClient.ExistsAsync())
                {


                    var sasBuilder = new BlobSasBuilder
                    {
                        BlobContainerName = containerClient.Name,
                        BlobName = blobName,
                        Resource = "b",
                        ExpiresOn = expiresOn
                    };

                    sasBuilder.SetPermissions(BlobSasPermissions.Read);
                    // Checks if the client can sign using Shared Key (Works in Azurite + Cloud)
                    if (blobClient.CanGenerateSasUri)
                    {
                        return blobClient.GenerateSasUri(sasBuilder).ToString();
                    }

                    var startsOn = DateTimeOffset.UtcNow.AddMinutes(-5);
                    var delegationKeyOptions = new BlobGetUserDelegationKeyOptions(expiresOn)
                    {
                        StartsOn = startsOn
                    };

                    var userDelegationKey = await _blobServiceClient.GetUserDelegationKeyAsync(delegationKeyOptions);

                    var sasToken = sasBuilder.ToSasQueryParameters(userDelegationKey.Value, _blobServiceClient.AccountName);
                    return $"{blobClient.Uri}?{sasToken}";
                }

                throw new FileNotFoundException("Blob not found.");
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error generating SAS URI");
                throw new Exception($"Error generating SAS URI: {ex.Message}");
            }
        }

        public async Task<IDictionary<string, string>> GetBlobMetadataAsync(string blobName, string containerName)
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                var blobClient = containerClient.GetBlobClient(blobName);

                BlobProperties properties = await blobClient.GetPropertiesAsync();

                return properties.Metadata;
            }
            catch (System.Exception)
            {

                throw;
            }
        }
    }
}