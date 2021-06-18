using System.Collections.Generic;

namespace Calypso.Api.Common
{
    public class PagedResult<T>
    {
        public int PageNumber { get; set; }
        public int ItemsPerPage { get; set; }
        public int TotalItems { get; set; }
        public IEnumerable<T> Items { get; set; }
    }
}