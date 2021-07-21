using System;
using System.ComponentModel.DataAnnotations;

namespace Calypso.Web.Models
{
    public class CreateFeedback
    {
        [Required]
        public string Subject { get; set; }
        [Required]
        public string Machine { get; set; }
        [Required]
        public string Location { get; set; }
        [Required]
        public string Reporter { get; set; }
        [Required]
        public string Role { get; set; }
        [Required]
        public DateTime Date { get; set; } = DateTime.UtcNow;
        [Required]
        public string Sbu { get; set; }
        [Required]
        public string ProjectName { get; set; }
        [Required]
        public string ProductName { get; set; }

        [Required] 
        public string Factory { get; set; } = "China";

        public byte[] FileContent { get; set; }
    }
}
