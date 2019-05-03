using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SampleWebsite.DataAccess
{
    public class User
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }


        public static User CreateDemoUser()
        {
            return new User()
            {
                EmailAddress = "demo@example.com",
                Password = "demo",
                FirstName = "Demo",
                LastName = "User",
                Id = Guid.NewGuid().ToString()
            };
        }
    }
}