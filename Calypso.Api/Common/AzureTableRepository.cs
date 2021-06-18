using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Options;

namespace Calypso.Api.Common
{
    public abstract class AzureTableRepository<T> where T : class, ITableEntity, new()
    {
        public abstract string TableName();

        private readonly TableClient _tableClient;
        protected AzureTableRepository(IOptions<AzureStorageOptions> options)
        {
            var tableServiceClient = new TableServiceClient(options.Value.ConnectionString);
            _ = tableServiceClient.CreateTableIfNotExists(TableName());
            _tableClient = new TableClient(options.Value.ConnectionString, TableName());
        }

        protected Task CreateEntityAsync(T entity)
        {
            return _tableClient.AddEntityAsync(entity);
        }

        protected Task UpdateEntityAsync(T entity)
        {
           return _tableClient.UpdateEntityAsync(entity, ETag.All);
        }

        protected Task DeleteEntityAsync(T entity)
        {
           return _tableClient.DeleteEntityAsync(entity.PartitionKey, entity.RowKey);
        }

        protected async Task<T> GetEntityAsync(string partitionKey, string rowKey)
        {
            var response = await _tableClient.GetEntityAsync<T>(partitionKey, rowKey);
            return response.Value;
        }
    }
}