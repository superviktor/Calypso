using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;

namespace Calypso.Api.Common
{
    public abstract class BlobStorageRepository : RepositoryBase
    {
        public abstract string ContainerName();

        private readonly BlobContainerClient _blobContainerClient;
        protected BlobStorageRepository(IOptions<AzureStorageOptions> options)
        {
            var blobServiceClient = new BlobServiceClient(options.Value.ConnectionString, new BlobClientOptions
            {
                Retry = {
                    Delay = Delay,     
                    MaxRetries = MaxRetries,                      
                    Mode = Mode,        
                    MaxDelay = MaxDelay
                }
            });
            _blobContainerClient = blobServiceClient.GetBlobContainerClient(ContainerName());
            if (!_blobContainerClient.Exists())
                _blobContainerClient.Create();
        }

        protected Task UploadAsync(string name, Stream content)
        {
            content.Position = 0;
            return _blobContainerClient.UploadBlobAsync(name, content);
        }

        protected async Task<Stream> DownloadAsync(string name)
        {
            var blobClient = _blobContainerClient.GetBlobClient(name);
            return (await blobClient.DownloadAsync()).Value.Content;
        }

        protected async Task DeleteAsync(string name)
        {
            var blobClient = _blobContainerClient.GetBlobClient(name);
            await blobClient.DeleteAsync();
        }
    }
}