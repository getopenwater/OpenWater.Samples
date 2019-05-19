using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

using SampleFunctions.Models;
using SampleFunctions.Extensions;
using SampleFunctions.Utilities;
using System.Net.Http.Headers;
using JWT;
using System.Collections.Generic;
using System;
using System.Text;

namespace SampleFunctions
{
    public static class PullData
    {
        public const string DOMAIN_NAME = "*** must get from openwater support ***";
        public const string LICENSE_KEY = "*** must get from openwater support ***";
        public const int REPORT_ID = 0; //must set from report id in report url

        [FunctionName("PullData")]
        public static async Task<HttpResponseMessage> PullDataTrigger([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            //pull all data collected
            var openwater = new OpenwaterUtility(DOMAIN_NAME, LICENSE_KEY);
            var data = openwater.DownloadReportJson<ScholarshipReport>(REPORT_ID);

            //push to my database
            foreach (var item in data.items)
                SimulateDatabasePush(item.Email, item.GPA, item.EssayMediaUrl, log);

            return req.Text("Done");
        }

        [FunctionName("PullInvoices")]
        public static async Task<HttpResponseMessage> PullInvoices([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            var openwater = new OpenwaterUtility(DOMAIN_NAME, LICENSE_KEY);

            var tenDaysAgo = DateTime.UtcNow.AddDays(-10);

            var invoiceData = openwater.GetInvoices(includeUnpaid: false, startDate: tenDaysAgo);

            var sb = new StringBuilder();

            foreach(var item in invoiceData)
            {
                sb.AppendLine(item.ToJson(prettyPrint: true));
            }

            return req.Text(sb.ToString());
        }


        private static void SimulateDatabasePush(string email, string gpa, string fileUrl, TraceWriter log)
        {
            //example code of pretending to push to a database
            log.Info($"Pushed {email} with gpa: {gpa} and url {fileUrl} to Database");
        }
    }

    public class ScholarshipReportItem
    {
        public string EssayMediaUrl { get; set; }
        public string ApplicationCode { get; set; }
        public string Email { get; set; }
        public string GPA { get; set; }
    }

    public class ScholarshipReport
    {
        public string headerinfo { get; set; }
        public List<ScholarshipReportItem> items { get; set; }
    }
}
