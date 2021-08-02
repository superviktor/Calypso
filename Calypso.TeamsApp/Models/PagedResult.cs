using System.Collections.Generic;

namespace Calypso.TeamsApp.Models
{
    public class PagedResult
    {
        public int PageNumber { get; set; }
        public int ItemsPerPage { get; set; }
        public int TotalItems { get; set; }
        public IEnumerable<Feedback> Items { get; set; }
    }
}