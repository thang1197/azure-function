using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using AzureFunction.Storage.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AzureFunction.Storage.Blob
{
    public class StorageBlobFunction(ILogger<StorageBlobFunction> logger)
    {
        private readonly ILogger<StorageBlobFunction> _logger = logger;

        [Function("BlobCreatedFunction")]
        [ServiceBusOutput("files", Connection = "AzureWebJobsServiceBus")]
        public async Task<FileMessageServiceBus> BlobCreated(
            [BlobTrigger("uploads/{name}")] BlockBlobClient blobClient,
            string name,
            FunctionContext context)
        {
            // Explicitly fetch the blob properties from Azure Storage
            var response = await blobClient.GetPropertiesAsync();
            var url = blobClient.Uri.AbsoluteUri;
            var properties = response.Value;

            string contentType = properties.ContentType
                ?? (properties.Metadata.TryGetValue("contentType", out var metaType) ? metaType : "application/octet-stream");

            string originalFileName = properties.Metadata.TryGetValue("fileName", out var origName) ? origName : name;
            int taskId = properties.Metadata.TryGetValue("taskId", out var taskIdStr) ? int.Parse(taskIdStr) : 0;
            int projectId = properties.Metadata.TryGetValue("projectId", out var projectIdStr) ? int.Parse(projectIdStr) : 0;
            string userCreated = properties.Metadata.TryGetValue("userCreated", out var userCreatedStr) ? userCreatedStr : string.Empty;
            string userUpdated = properties.Metadata.TryGetValue("userUpdated", out var userUpdatedStr) ? userUpdatedStr : string.Empty;

            _logger.LogInformation("Blob trigger function Processed blob\n Name:{Name} \n Size: {Size} \n Content type: {ContentType}", 
            name, properties.ContentLength, contentType);
            var fileMessage = new FileMessageServiceBus
            {
                FileId = Random.Shared.Next(1, 1000),
                FileName = blobClient.Name,
                FilePath = $"uploads/{name}",
                FileType = contentType,
                ProjectId = projectId,
                TaskId = taskId,
                UserCreated = userCreated,
                UserUpdated = userUpdated,
                Url = url,
            };

            return fileMessage;
        }
    }
}