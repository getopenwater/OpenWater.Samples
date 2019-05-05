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
            var result = fakeDb.Where(c => c.Color.ToLower().Contains(search.ToLower())).ToArray();
            var response = req.Json(result, prettyPrint: true);
            response.EnableCors();
            return response;
        }

        private static List<LookupTestItem> FakeDatabase()
        {
            var db = new List<LookupTestItem>();

            db.Add(new LookupTestItem { Color = "Blue", HexValue = "" });
            db.Add(new LookupTestItem { Color = "Orange", HexValue = "" });
            db.Add(new LookupTestItem { Color = "Green", HexValue = "" });
            db.Add(new LookupTestItem { Color = "Red", HexValue = "" });
            db.Add(new LookupTestItem { Color = "Cyan", HexValue = "" });
            db.Add(new LookupTestItem { Color = "Magenta", HexValue = "" });
            db.Add(new LookupTestItem { Color = "Purple", HexValue = "" });
            db.Add(new LookupTestItem { Color = "Pink", HexValue = "" });
            db.Add(new LookupTestItem { Color = "Yellow", HexValue = "" });

            return db;
        }

        public class LookupTestItem
        {
            public string Color { get; set; }
            public string HexValue { get; set; }
        }
    }
}
