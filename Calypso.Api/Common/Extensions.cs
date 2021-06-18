using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Calypso.Api.Common
{
    public static class Extensions
    {
        public static async Task<Stream> ToStreamAsync(this IFormFile file)
        {
            var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            return stream;
        }
    }
}