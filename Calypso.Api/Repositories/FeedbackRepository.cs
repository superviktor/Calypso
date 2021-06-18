using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Calypso.Api.Common;
using Calypso.Api.Models;
using Microsoft.Extensions.Options;

namespace Calypso.Api.Repositories
{
    public sealed class FeedbackRepository : AzureTableRepository<Feedback>, IFeedbackRepository
    {
        public FeedbackRepository(IOptions<AzureStorageOptions> options) : base(options)
        {
        }

        public override string TableName() => "Feedback";

        public string PartitionKey => "Feedback";

        public Task<IEnumerable<Feedback>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Feedback> GetAsync(string rowKey)
        {
            return GetEntityAsync(PartitionKey, rowKey);
        }

        public Task CreateAsync(Feedback feedback)
        {
            return CreateEntityAsync(feedback);
        }

        public Task UpdateAsync(Feedback feedback)
        {
            return UpdateEntityAsync(feedback);
        }

        public Task DeleteAsync(Feedback feedback)
        {
            return DeleteEntityAsync(feedback);
        }
    }
}