using System;
using Calypso.Api.Enums;

namespace Calypso.Api.Dtos
{
    public class FeedbackDto
    {
        public string RowKey { get; set; }
        public int Number { get; set; }
        public string Description { get; set; }
        public string Subject { get; set; }
        public string Machine { get; set; }
        public string Location { get; set; }
        public string Reporter { get; set; }
        public string Role { get; set; }
        public DateTime Date { get; set; }
        public string Sbu { get; set; }
        public string ProjectName { get; set; }
        public string ProductName { get; set; }
        public Factory Factory { get; set; }
        public string FileName { get; set; }
    }
}