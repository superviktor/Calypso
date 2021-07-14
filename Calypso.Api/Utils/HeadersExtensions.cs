using Microsoft.AspNetCore.Http;

namespace Calypso.Api.Utils
{
    public static class HeadersExtensions
    {
        public static string GetAuthorizationHeaderValue(this IHeaderDictionary dictionary)
        {
            return dictionary["Authorization"].ToString()["Bearer ".Length..].Trim();
        }
    }
}