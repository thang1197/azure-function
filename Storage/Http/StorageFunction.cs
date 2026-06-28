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
        public async Task<IActionResult> UploadFile([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            try
            {
                // Make sure the request is multipart/form-data
                if (!req.HasFormContentType)
                {
                    return HttpHelper.GenerateErrorResponse("Request must be multipart/form-data", StatusCodes.Status400BadRequest);
                }

                var form = await req.ReadFormAsync();
                var file = form.Files["file"]; // "file" must match the form-data field name

                if (file == null || file.Length == 0)
                {
                    return HttpHelper.GenerateErrorResponse("No file uploaded", StatusCodes.Status400BadRequest);
                }

                var fileName = await _blobStorageService.UploadAsync(file.FileName, file.OpenReadStream());
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
        public async Task<IActionResult> DownloadFile([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
        {
            // Implementation for the storage function
            return new OkResult();
        }

        [Function("DeleteFileFunction")]
        public async Task<IActionResult> DeleteFile([HttpTrigger(AuthorizationLevel.Function, "delete")] HttpRequest req)
        {
            // Implementation for the storage function
            return new OkResult();
        }

        [Function("ListFilesFunction")]
        public async Task<IActionResult> ListFiles([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req, CancellationToken cancellationToken)
        {
            try
            {   
                var files = await _blobStorageService.ListFilesAsync();
                return HttpHelper.GenerateResponse(files);
            }
            catch (System.Exception ex)
            {
                
                return HttpHelper.GenerateErrorResponse(ex.Message, StatusCodes.Status500InternalServerError);
            }
        }

        [Function("GenerateSasUriFunction")]
        public async Task<IActionResult> GenerateSasUri([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
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
    }
}