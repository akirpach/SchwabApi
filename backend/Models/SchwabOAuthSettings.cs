using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Models
{
    public class SchwabOAuthSettings
    {
        public required string ClientId { get; set; }
        public required string ClientSecret { get; set; }
        public string? RedirectUri { get; set; }
        public string? TokenUrl { get; set; }
        public string? AuthorizeUrl { get; set; }
    }
}