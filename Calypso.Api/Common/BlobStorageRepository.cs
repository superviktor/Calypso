using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.Extensions.Options;

namespace Calypso.Api.Common
{
    public abstract class BlobStorageRepository : RepositoryBase
    {
        private readonly IOptions<AzureStorageOptions> _options;
        public abstract string ContainerName();

        private readonly BlobContainerClient _blobContainerClient;
        protected BlobStorageRepository(IOptions<AzureStorageOptions> options)
        {
            _options = options;
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

        protected Task<string> GetSharedUrlWithSas(string name)
        {
            var blobSasBuilder = new BlobSasBuilder
            {
                StartsOn = DateTime.UtcNow.AddDays(-1),
                ExpiresOn = DateTime.UtcNow.AddDays(30),
                BlobContainerName = ContainerName(),
                BlobName = name
            };
            blobSasBuilder.SetPermissions(BlobSasPermissions.Read);
            var storageSharedKeyCredential = new StorageSharedKeyCredential(_options.Value.AccountName, _options.Value.AccountKey);
            var sasQueryParameters = blobSasBuilder.ToSasQueryParameters(storageSharedKeyCredential);
            var fullUri = new UriBuilder
            {
                Scheme = "https",
                Host = $"{_options.Value.AccountName}.blob.core.windows.net",
                Path = $"{ContainerName()}/{name}",
                Query = sasQueryParameters.ToString()
            };

            var result = fullUri.ToString();
            return Task.FromResult(result);
        }
    }
}