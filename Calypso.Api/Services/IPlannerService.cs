using System.Threading.Tasks;

namespace Calypso.Api.Services
{
    public interface IPlannerService
    {
        Task CreateTask(string authToken, string title);
    }
}