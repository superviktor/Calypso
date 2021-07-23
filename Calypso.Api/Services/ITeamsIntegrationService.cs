using System.Threading.Tasks;

namespace Calypso.Api.Services
{
    public interface ITeamsIntegrationService
    {
        Task<string> CreateTask(string authToken, string title);
        Task AddTaskDetails(string authToken, string taskId, string description, string attachmentUrl = null);
        Task SendChannelMessage(string authToken, string taskTitle);
    }
}