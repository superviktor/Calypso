using System.Threading.Tasks;
using Calypso.Api.Common;
using Calypso.Api.Models;

namespace Calypso.Api.Repositories
{
    public interface IFeedbackRepository
    {
        string PartitionKey { get; }
        Task<PagedResult<Feedback>> GetAsync(int pageNumber, int itemsPerPage, string searchString = null);
        Task<Feedback> GetAsync(string rowKey);
        Task CreateAsync(Feedback feedback);
        Task UpdateAsync(Feedback feedback);
        Task DeleteAsync(Feedback feedback);
        Task<int> GetNextNumber();
    }
}