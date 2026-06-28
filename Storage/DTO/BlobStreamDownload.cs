namespace AzureFunction.Storage.DTO
{
    public class BlobStreamDownload
    {
        public required Stream Content { get; set; }
        public required string ContentType { get; set; }
        public required string FileName { get; set; }
    }
}