using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace ProtoProject.API
{
    public class BlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly IConfiguration _configuration;
        private readonly string connectionString; 
        public BlobStorageService(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = _configuration["AzureStorage:ConnectionString"];
            _blobServiceClient = new BlobServiceClient(connectionString);
        }

        public async Task<string> UploadFileAsync(string containerName, string blobName, Stream fileStream)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.BlobContainer);

            var blobClient = containerClient.GetBlobClient(blobName);
            await blobClient.UploadAsync(fileStream);

            return blobClient.Uri.ToString();
        }

        public async Task<Stream> DownloadFileAsync(string containerName, string blobName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            if (!await containerClient.ExistsAsync())
            {
                throw new ArgumentException($"Container '{containerName}' does not exist.");
            }

            var blobClient = containerClient.GetBlobClient(blobName);
            if (!await blobClient.ExistsAsync())
            {
                throw new ArgumentException($"Blob '{blobName}' does not exist in container '{containerName}'.");
            }

            var response = await blobClient.DownloadAsync();
            return response.Value.Content;
        }

        public async Task DeleteFileAsync(string containerName, string blobName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            await blobClient.DeleteAsync();
        }
    }

}
