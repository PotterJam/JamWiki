using System;

namespace WikiApi
{
    public class Wiki
    {
        public Guid Id { get; }
        public string Name { get; }
        public string Body { get; }
        public string[] Tags { get; }

        public Wiki(Guid id, string name, string body, string[] tags)
        {
            Id = id;
            Name = name;
            Tags = tags;
            Body = body;
        }
    }
}