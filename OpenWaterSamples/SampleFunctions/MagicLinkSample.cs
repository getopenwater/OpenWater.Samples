using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using SampleFunctions.Extensions;

namespace SampleFunctions
{
    /// <summary>
    /// Video Explaination: http://somup.com/cr1rF3qdeq
    /// </summary>
    public static class MagicLinkSample
    {
        [FunctionName("MagicLinkTest")]
        public static async Task<HttpResponseMessage> MagicLinkTest([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            var email = req.GetQueryNameValuePairs().Where(c => c.Key.ToLower() == "email").First().Value.ToLower();
            var db = MockDatabase();

            if (db.Any(c => c.Email.ToLower() == email))
                return req.Json(db.First(c => c.Email.ToLower() == email));

            return req.Json(new ExternalStorageUserInfo { IsValid = false });
            
        }

        [FunctionName("MagicLinkSampleAllAccess")]
        public static async Task<HttpResponseMessage> MagicLinkSampleAllAccess([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {            
            var retValue = new ExternalStorageUserInfo
            {
                IsValid = true
            };

            return req.Json(retValue);
        }

        public static List<MockDbItem> MockDatabase()
        {
            var mockDb = new List<MockDbItem>();
            mockDb.Add(new MockDbItem
            {
                IsValid = true,
                FirstName = "Demo",
                LastName = "Attendee",
                Company = "Sample Co",
                Email = "demo@mailinator.com"
            });

            mockDb.Add(new MockDbItem
            {
                IsValid = true,
                FirstName = "Demo 2",
                LastName = "Attendee 2",
                Company = "Sample Co 2",
                Email = "demo2@mailinator.com"
            });

            return mockDb;
        }


        public class ExternalStorageUserInfo 
        {
            public bool IsValid { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Company { get; set; }
            public string ThirdPartyCorporateId { get; set; }
            public string ExternalAuthData { get; set; }            
        }

        public class MockDbItem : ExternalStorageUserInfo
        {
            public string Email { get; set; }
        }
    }
}
