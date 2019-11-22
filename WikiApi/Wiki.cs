using System;

namespace WikiApi
{
    public class Wiki
    {
        public Guid id { get; }
        public string name { get; }
        public string[] tags { get; }

        public Wiki(Guid id, string name, string[] tags)
        {
            this.id = id;
            this.name = name;
            this.tags = tags;
        }
    }
}