using System;

namespace JamWiki.Api
{
    public class WikiUser
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Issuer { get; set; }
        
    }
}