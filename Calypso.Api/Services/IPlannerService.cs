using System.Threading.Tasks;

namespace Calypso.Api.Services
{
    public interface IPlannerService
    {
        Task<string> CreateTask(string authToken, string title);
        Task AddTaskDescription(string authToken, string taskId, string description);
    }
}