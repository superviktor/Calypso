﻿using System.IO;
using System.Threading.Tasks;
using Calypso.Api.Common;
using Microsoft.Extensions.Options;

namespace Calypso.Api.Repositories
{
    public class FeedbackImageRepository: BlobStorageRepository, IFeedbackImageRepository
    {
        public FeedbackImageRepository(IOptions<AzureStorageOptions> options) : base(options)
        {
        }

        public override string ContainerName() => "feedback-image";

        public Task UploadImageAsync(string name, Stream content)
        {
            return UploadAsync(name, content);
        }

        public Task<Stream> DownloadImageAsync(string name)
        {
            return DownloadAsync(name);
        }
    }
}