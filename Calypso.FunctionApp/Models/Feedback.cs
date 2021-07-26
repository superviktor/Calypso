using System;
using Azure;
using Azure.Data.Tables;

namespace Calypso.FunctionApp.Models
{
    public class Feedback : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public int Number { get; set; }
        public string Description { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
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
        public string FileName { get; set; }
        public string TaskId { get; set; }
        public string Status { get; set; }
    }
}