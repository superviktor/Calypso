using System.Linq;
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

        public async Task<PagedResult<Feedback>> GetAsync(int pageNumber, int itemsPerPage, string searchString = null)
        {
            var feedbacks = await GetEntitiesAsync();

            var filtered = !string.IsNullOrWhiteSpace(searchString) 
                ? feedbacks.Where(x => 
                    x.Subject.Contains(searchString) &&
                    x.ProjectName.Contains(searchString) &&
                    x.Factory.ToString().Contains(searchString) &&
                    x.Location.Contains(searchString) &&
                    x.Machine.Contains(searchString) &&
                    x.ProductName.Contains(searchString) &&
                    x.Reporter.Contains(searchString) &&
                    x.Sbu.Contains(searchString) &&
                    x.Role.Contains(searchString)
                    ).ToList() 
                : feedbacks;

            var pagedItems = filtered
                .Skip((pageNumber - 1) * itemsPerPage)
                .Take(itemsPerPage);

            return new PagedResult<Feedback>
            {
                PageNumber = pageNumber,
                ItemsPerPage = itemsPerPage,
                TotalItems = filtered.Count,
                Items = pagedItems
            };
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