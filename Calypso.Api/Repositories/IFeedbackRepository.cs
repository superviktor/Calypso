using System.Collections.Generic;
using System.Threading.Tasks;
using Calypso.Api.Models;

namespace Calypso.Api.Repositories
{
    public interface IFeedbackRepository
    {
        string PartitionKey { get; }
        Task<IEnumerable<Feedback>> GetAllAsync();
        Task<Feedback> GetAsync(string rowKey);
        Task CreateAsync(Feedback feedback);
        Task UpdateAsync(Feedback feedback);
        Task DeleteAsync(Feedback id);
    }
}