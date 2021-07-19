using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Calypso.Console
{
    class Program
    {
        static async Task Main()
        {
            var http = new HttpClient();
            var requestUri = new Uri("http://localhost:5000/api/Feedback?pageNumber=1&itemsPerPage=10");
            var response = await http.GetAsync(requestUri);
            var content = await response.Content.ReadAsStringAsync();
            var pagedResult = JsonSerializer.Deserialize<PagedResult>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            var feedbacks = pagedResult.Items.ToArray();
            System.Console.WriteLine(content);
        }
    }

    public class Feedback
    {
        public string Subject { get; set; }
        public string Machine { get; set; }
        public string Location { get; set; }
        public string Reporter { get; set; }
        public string Role { get; set; }
        public string Date { get; set; }
        public string Sbu { get; set; }
        public string ProjectName { get; set; }
        public string ProductName { get; set; }
        public string Factory { get; set; }
    }

    public class PagedResult
    {
        public int PageNumber { get; set; }
        public int ItemsPerPage { get; set; }
        public int TotalItems { get; set; }
        public IEnumerable<Feedback> Items { get; set; }
    }
}