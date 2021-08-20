using System.Linq;
using System.Threading.Tasks;
using Calypso.TeamsApp123.Common;
using Calypso.TeamsApp123.Models;
using Microsoft.Extensions.Options;

namespace Calypso.TeamsApp123.Repositories
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
                    !string.IsNullOrWhiteSpace(x.Subject) && x.Subject.Contains(searchString) ||
                    !string.IsNullOrWhiteSpace(x.ProjectName) && x.ProjectName.Contains(searchString) ||
                    !string.IsNullOrWhiteSpace(x.Factory.ToString()) && x.Factory.ToString().Contains(searchString) ||
                    !string.IsNullOrWhiteSpace(x.Location) && x.Location.Contains(searchString) ||
                    !string.IsNullOrWhiteSpace(x.Machine) && x.Machine.Contains(searchString) ||
                    !string.IsNullOrWhiteSpace(x.ProductName) && x.ProductName.Contains(searchString) ||
                    !string.IsNullOrWhiteSpace(x.Reporter) && x.Reporter.Contains(searchString) ||
                    !string.IsNullOrWhiteSpace(x.Sbu) && x.Sbu.Contains(searchString) ||
                    !string.IsNullOrWhiteSpace(x.Role) && x.Role.Contains(searchString)
                ).ToList()
                : feedbacks;

            var pagedItems = filtered
                .OrderByDescending(f => f.Timestamp)
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

        public async Task<int> GetNextNumber()
        {
            var feedbacks = await GetEntitiesAsync();
            if (feedbacks.Count == 0)
                return 1;
            return feedbacks.Max(f => f.Number) + 1;
        }
    }
}