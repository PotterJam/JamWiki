using System;

namespace WikiApi
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Issuer { get; set; }

        public User()
        {
            
        }
    }
}