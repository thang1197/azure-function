using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureFunction.Storage.DTO
{
    public class DownloadFileRequest
    {
        public required string FileName { get; set; }

        public required string ContainerName { get; set; }
    }
}