namespace Calypso.Web.Models
{
    public class Feedback
    {
        public string RowKey { get; set; }
        public int Number { get; set; }
        public string Description { get; set; }
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
    }
}