using System.Text.Json;
using Azure;
using AzureFunction.Storage.DTO;
using AzureFunction.Storage.Service;
using AzureFunction.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AzureFunction.Storage.Http
{
    public class StorageFunction(IBlobStorageService blobStorageService, ILogger<StorageFunction> logger)
    {
        private readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        private readonly IBlobStorageService _blobStorageService = blobStorageService;
        private readonly ILogger<StorageFunction> _logger = logger;
        [Function("UploadFileFunction")]
        public async Task<IActionResult> UploadFile([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req, FunctionContext executionContext)
        {
            try
            {
                var claims = executionContext.GetUser();
                _logger.LogInformation($"claims: {claims}");
                // Make sure the request is multipart/form-data
                if (!req.HasFormContentType)
                {
                    return HttpHelper.GenerateErrorResponse("Request must be multipart/form-data", StatusCodes.Status400BadRequest);
                }

                var form = await req.ReadFormAsync();
                var file = form.Files["file"]; // "file" must match the form-data field name
                var containerName = form["containerName"];

                if(string.IsNullOrEmpty(containerName))
                {
                    return HttpHelper.GenerateErrorResponse("Missing container name", StatusCodes.Status400BadRequest);
                }

                if (file == null || file.Length == 0)
                {
                    return HttpHelper.GenerateErrorResponse("No file uploaded", StatusCodes.Status400BadRequest);
                }

                var metadata = new Dictionary<string, string>
                {
                    {"userCreated", claims != null ? claims.UserId.ToString() : "default" },
                    {"userUpdated", claims != null ? claims.UserId.ToString() : "default" }
                };
                var fileName = await _blobStorageService.UploadAsync(file.FileName, containerName, file.OpenReadStream(), metadata);
                return HttpHelper.GenerateResponse(fileName);
            }
            catch (RequestFailedException ex)
            {
                return HttpHelper.GenerateErrorResponse(ex.Message, ex.Status);
            }
            catch (Exception ex)
            {
                return HttpHelper.GenerateErrorResponse(ex.Message, StatusCodes.Status500InternalServerError);   
            }
        }

        [Function("DownloadFileFunction")]
        public async Task<IActionResult> DownloadFile([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
        {
            // Implementation for the storage function
            return new OkResult();
        }

        [Function("DeleteFileFunction")]
        public async Task<IActionResult> DeleteFile([HttpTrigger(AuthorizationLevel.Anonymous, "delete")] HttpRequest req)
        {
            try
            {
                var blobName = req.Query["blobName"];
                var containerName = req.Query["containerName"];

                if(string.IsNullOrEmpty(blobName) || string.IsNullOrEmpty(containerName))
                {
                    return HttpHelper.GenerateErrorResponse("Missing file name or container name", StatusCodes.Status400BadRequest);
                }

                await _blobStorageService.DeleteAsync(blobName, containerName);
                return HttpHelper.GenerateResponse("File deleted successfully.");
            }
            catch (System.Exception ex)
            {
                
                return HttpHelper.GenerateErrorResponse(ex.Message, StatusCodes.Status500InternalServerError);
            }
        }

        [Function("ListFilesFunction")]
        public async Task<IActionResult> ListFiles([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req, CancellationToken cancellationToken)
        {
            try
            {   
                var containerName = req.Query["containerName"];

                if(string.IsNullOrEmpty(containerName))
                {
                    return HttpHelper.GenerateErrorResponse("Missing container name", StatusCodes.Status400BadRequest);
                }
                var files = await _blobStorageService.ListFilesAsync(containerName);
                return HttpHelper.GenerateResponse(files);
            }
            catch (System.Exception ex)
            {
                
                return HttpHelper.GenerateErrorResponse(ex.Message, StatusCodes.Status500InternalServerError);
            }
        }

        [Function("GenerateSasUriFunction")]
        public async Task<IActionResult> GenerateSasUri([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
        {
            try
            {
                // var sasUriRequest = JsonSerializer.Deserialize<SasUriRequest>(req.Body, _jsonSerializerOptions);
                
                string file = req.Query["file"];
                _logger.LogInformation($"file: {file}");
                if(string.IsNullOrEmpty(file))
                {
                    return HttpHelper.GenerateErrorResponse("Missing file name", StatusCodes.Status400BadRequest);
                }

                var uri = await _blobStorageService.GenerateSasUri(file);
                return HttpHelper.GenerateResponse(uri);
            }
            catch (System.Exception ex)
            {
                
                return HttpHelper.GenerateErrorResponse(ex.Message, StatusCodes.Status500InternalServerError);
            }
        }

        [Function("GetBlobMetadataFunction")]
        public async Task<IActionResult> GetBlobMetadata([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
        {
            try
            {
                var containerName = req.Query["containerName"];
                var blobName = req.Query["blobName"];

                if(string.IsNullOrEmpty(containerName) || string.IsNullOrEmpty(blobName))
                {
                    return HttpHelper.GenerateErrorResponse("Missing container name or blob name", StatusCodes.Status400BadRequest);
                }

                var metadata = await _blobStorageService.GetBlobMetadataAsync(blobName, containerName);
                return HttpHelper.GenerateResponse(metadata);
            }
            catch (System.Exception ex)
            {              
               return HttpHelper.GenerateErrorResponse(ex.Message, StatusCodes.Status500InternalServerError);
            }
        }
    }
}