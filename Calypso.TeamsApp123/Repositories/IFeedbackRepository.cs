using System.Threading.Tasks;
using Calypso.TeamsApp123.Common;
using Calypso.TeamsApp123.Models;

namespace Calypso.TeamsApp123.Repositories
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