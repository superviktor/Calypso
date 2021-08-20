using System.Threading.Tasks;

namespace Calypso.TeamsApp123.Services
{
    public interface ITeamsIntegrationService
    {
        Task<string> CreateTask(string token, string title);
        Task AddTaskDetails(string token, string taskId, string description, string attachmentUrl = null);
        Task SendChannelMessage(string token, string taskTitle);
    }
}