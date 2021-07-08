using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Options;

namespace Calypso.Api.Common
{
    public abstract class AzureTableRepository<T>: RepositoryBase where T : class, ITableEntity, new()
    {
        public abstract string TableName();

        private readonly TableClient _tableClient;
        protected AzureTableRepository(IOptions<AzureStorageOptions> options)
        {
            var tableServiceClient = new TableServiceClient(options.Value.ConnectionString, new TableClientOptions
            {
                Retry =
                {
                    Delay = Delay,
                    MaxRetries = MaxRetries,
                    Mode = Mode,
                    MaxDelay = MaxDelay
                }
            });
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
            T result;
            try
            {
                result = await _tableClient.GetEntityAsync<T>(partitionKey, rowKey);
            }
            catch (RequestFailedException e)
            {
                result = null;
            }

            return result;
        }

        protected Task<List<T>> GetEntitiesAsync()
        {
            return Task.FromResult(_tableClient.Query<T>().ToList());
            //var items = _tableClient.Query<T>()
            //    .Skip((pageNumber -1) * itemsPerPage)
            //    .Take(itemsPerPage);
            //return Task.FromResult(new PagedResult<T>
            //{
            //    PageNumber = pageNumber,
            //    ItemsPerPage = itemsPerPage,
            //    TotalItems = .Count(),
            //    Items = items
            //});
        }
    }
}