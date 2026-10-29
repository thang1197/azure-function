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
        public Task<FileStorage> UploadAsync(string fileName, string containerName, Stream fileContent, IDictionary<string, string>? metaData = null);

        public Task<BlobStreamDownload> DownloadAsync(string blobName, string containerName);

        public Task<bool> DeleteAsync(string blobName, string containerName);

        public Task<List<string>> ListFilesAsync(string containerName);

        public Task<string> GenerateSasUri(string blobName);

        public Task<IDictionary<string, string>> GetBlobMetadataAsync(string blobName, string containerName);
    }
}