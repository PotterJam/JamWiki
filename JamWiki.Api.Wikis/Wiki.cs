using System;
using System.Collections.Generic;

namespace JamWiki.Api
{
    public class Wiki
    {
        public Guid Id { get; }
        public string Name { get; }
        public string Body { get; }
        public IEnumerable<string> Tags { get; }

        public Wiki(Guid id, string name, string body, IEnumerable<string> tags)
        {
            Id = id;
            Name = name;
            Tags = tags;
            Body = body;
        }
    }
}