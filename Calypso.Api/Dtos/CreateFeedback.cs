﻿using System;
using Calypso.Api.Enums;
using Microsoft.AspNetCore.Http;

namespace Calypso.Api.Dtos
{
    public class CreateFeedback
    {
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
        public IFormFile File { get; set; }
    }
}