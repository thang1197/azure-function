using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureFunction.Storage.DTO;
using AzureFunction.Storage.Entity;
using Microsoft.AspNetCore.Http;

namespace AzureFunction.Storage.Service
{
    public interface IBlobStorageService
    {
        public Task<FileStorage> UploadAsync(string fileName, Stream fileContent);

        public Task<BlobStreamDownload> DownloadAsync(string blobName);

        public Task<bool> DeleteAsync(string blobName);

        public Task<List<string>> ListFilesAsync();

        public Task<string> GenerateSasUri(string blobName);
    }
}