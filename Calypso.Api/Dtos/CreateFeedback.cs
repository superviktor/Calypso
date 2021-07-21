using System;
using System.ComponentModel.DataAnnotations;
using Calypso.Api.Enums;
using Calypso.Api.Utils;
using Microsoft.AspNetCore.Http;

namespace Calypso.Api.Dtos
{
    public class CreateFeedback
    {
        [Required]
        public string Description { get; set; }
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
        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; }
        [Required]
        public string Sbu { get; set; }
        [Required]
        public string ProjectName { get; set; }
        [Required]
        public string ProductName { get; set; }
        [Required]
        public Factory Factory { get; set; }
        [MaxFileSize(1 * 1024 * 1024)]
        [AllowedExtensions(new [] { ".jpg" })]
        public IFormFile File { get; set; }
    }
}