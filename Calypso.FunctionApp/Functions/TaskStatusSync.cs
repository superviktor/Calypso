using System.Threading.Tasks;
using Calypso.FunctionApp.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace Calypso.FunctionApp.Functions
{
    public class TaskStatusSync
    {
        private readonly ISyncService _syncService;

        public TaskStatusSync(ISyncService syncService)
        {
            _syncService = syncService;
        }

        [FunctionName("TaskCheck")]
        public async Task Run([TimerTrigger("* * * * *")]TimerInfo myTimer, ILogger log)
        {
            await _syncService.SyncStatuses();
        }
    }
}
