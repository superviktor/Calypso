using System.IO;
using System.Threading.Tasks;

namespace Calypso.Api.Repositories
{
    public interface IFeedbackImageRepository
    {
        Task UploadImageAsync(string name, Stream content);
        Task<Stream> DownloadImageAsync(string name);
        Task DeleteImageAsync(string name);
        Task<string> GetSharedUrl(string name);
    }
}
