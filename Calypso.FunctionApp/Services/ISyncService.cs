using System.Threading.Tasks;

namespace Calypso.FunctionApp.Services
{
    public interface ISyncService
    {
        Task SyncStatuses();
    }
}