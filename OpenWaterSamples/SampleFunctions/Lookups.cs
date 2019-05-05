using System;
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
    public static class Lookups
    {
        [FunctionName("Lookup")]
        public static async Task<HttpResponseMessage> Lookup([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "lookup")]HttpRequestMessage req, TraceWriter log)
        {
            var search = req.GetQueryNameValuePairs().Where(c => c.Key.ToLower() == "search").Select(c => c.Value).FirstOrDefault();
            if (search == null)
                throw new Exception("Search parameter is required");

            var fakeDb = FakeDatabase();
            var result = fakeDb.Where(c => c.FullName.ToLower().Contains(search.ToLower())).ToArray();
            var response = req.Json(result, prettyPrint: true);
            response.EnableCors();
            return response;
        }

        private static List<LookupTestItem> FakeDatabase()
        {
            var db = new List<LookupTestItem>();

            db.Add(new LookupTestItem { FirstName = "John", LastName = "Doe", University = "Florida State University"});
            db.Add(new LookupTestItem { FirstName = "Jane", LastName = "Doe", University = "Ohio State University" });
            db.Add(new LookupTestItem { FirstName = "Bill", LastName = "Gates", University = "Harvard" });
            db.Add(new LookupTestItem { FirstName = "Mark", LastName = "Zuckerberg", University = "Harvard" });
            db.Add(new LookupTestItem { FirstName = "Larry", LastName = "Ellison", University = "University of California, Berkeley" });
            db.Add(new LookupTestItem { FirstName = "Michelle", LastName = "Obama", University = "Princeton" });
            db.Add(new LookupTestItem { FirstName = "Oprah", LastName = "Winfrey", University = "None" });

            return db;
        }

        public class LookupTestItem
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string University { get; set; }

            public string FullName => FirstName + " " + LastName;
        }
    }
}
