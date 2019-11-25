using System;

namespace WikiApi
{
    public class Wiki
    {
        public Guid Id { get; }
        public string Name { get; }
        public string[] Tags { get; }

        public Wiki(Guid id, string name, string[] tags)
        {
            this.Id = id;
            this.Name = name;
            this.Tags = tags;
        }
    }
}