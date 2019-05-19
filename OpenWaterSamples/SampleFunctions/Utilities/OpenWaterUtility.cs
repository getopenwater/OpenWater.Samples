using SampleFunctions.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using Microsoft.CSharp;

namespace SampleFunctions.Utilities
{
    public class OpenwaterUtility
    {
        private string _domain;
        private Guid _licenseKey;

        public OpenwaterUtility(string domain, Guid licenseKey)
        {
            _domain = domain;
            _licenseKey = licenseKey;
        }

        public OpenwaterUtility(string domain, string licenseKey) : this(domain, new Guid(licenseKey))
        {
            //Example of constructor overloading, this() will be called before this section
        }

        public IEnumerable<Invoice> GetInvoices(bool includeUnpaid = false, DateTime? startDate = null, DateTime? endDate = null)
        {
            using (var client = new HttpClient())
            {
                // Append some custom header
                client.DefaultRequestHeaders.Add("licenseKey", _licenseKey.ToString());

                //Create Appropriate URL for GET Request
                var url = $"https://{_domain}/a/admin/Api/GetInvoices?includeUnpaid={includeUnpaid}";
                if (startDate.HasValue)
                    url += "&invoiceAtStart=" + startDate.Value.ToShortDateString();

                if (endDate.HasValue)
                    url += "&invoiceAtEnd=" + endDate.Value.ToShortDateString();


                var result = client.GetAsync(url).Result.ReadContentAs<IEnumerable<Invoice>>();
                return result;
            }
        }

        public enum ReportType
        {
            Application = 0,
            Judge = 1,
            Session = 2
        }

        public enum OutputFormat
        {
            Json = 0,
            Csv = 1,
            Xls = 2
        }

        public string GetReportFileUrl(int reportId, OutputFormat outputFormat = OutputFormat.Json, ReportType reportType = ReportType.Application, string orgCode = null)
        {
            var reportTypeString = "application";
            if (reportType == ReportType.Judge)
                reportTypeString = "judge";
            else if (reportType == ReportType.Session)
                reportTypeString = "session";

            var outputFormatString = "json";

            if (outputFormat == OutputFormat.Csv)
                outputFormatString = "csv";
            else if (outputFormat == OutputFormat.Xls)
                outputFormatString = "xls";


            using (var client = new HttpClient())
            {
                var url = "";
                if (orgCode == null)
                    url = $"https://{_domain}/a/admin/Api/BeginReportJob?reportId={reportId}&outputFormat={outputFormatString}&reportType={reportTypeString}";
                else
                    url = $"https://{_domain}/a/admin/organizations/{orgCode}/Api/BeginReportJob?reportId={reportId}&outputFormat={outputFormatString}&reportType={reportTypeString}";

                // Append some custom header
                client.DefaultRequestHeaders.Add("licenseKey", _licenseKey.ToString());
                dynamic jobResult = client.Post(url, new StringContent("")).ReadContentAs<dynamic>();

                var stopWatch = new Stopwatch();
                stopWatch.Start();

                while (stopWatch.Elapsed.TotalSeconds < 600) //10 minutes
                {
                    try
                    {
                        var jobUrl = $"https://{_domain}/a/admin/Api/GetReportJobStatus?jobId={jobResult.result.jobId}";
                        var jobReadyResult = client.Get(jobUrl).ReadContentAs<dynamic>();

                        if (jobReadyResult.result.status == CronJobStatus.Completed)
                        {
                            return (string)jobReadyResult.result.result.fileUrl;
                        }
                        else if (jobReadyResult.result.status == CronJobStatus.Failed)
                        {
                            throw new Exception("Report Failed");
                        }
                    }
                    catch
                    {
                        System.Threading.Thread.Sleep(25000);
                        //Ignore HTTP Exceptions, if the first request worked
                    }

                    System.Threading.Thread.Sleep(5000);
                }

                throw new Exception("Timeout");
            }

        }

        public T DownloadReportJson<T>(int reportId, ReportType reportType = ReportType.Application, string orgCode = null) where T : class
        {
            var file = GetReportFileUrl(reportId, OutputFormat.Json, reportType, orgCode);

            using (var client = new HttpClient())
            {
                return client.Get(file).ReadContentAs<T>();
            }
        }


        #region Api Helper Classes
        enum CronJobStatus
        {
            None,
            Requested,
            InProgress,
            Completed,
            Failed
        }

        class ReportResult
        {
            public int JobId { get; set; }
            public CronJobStatus Status { get; set; }
            public string Result { get; set; }
        }

        public class BillingLineItem
        {
            public int accountingTransactionId { get; set; }
            public double amount { get; set; }
            public string notes { get; set; }
            public string details { get; set; }
            public bool isManualAdjustment { get; set; }
            public bool isVat { get; set; }
            public int? targetType { get; set; }
            public string createdIn { get; set; }
            public int? deletedApplicationId { get; set; }
            public int id { get; set; }
            public string createdAt { get; set; }
            public DateTime? updatedAt { get; set; }
        }

        public class Payment
        {
            public int accountingTransactionId { get; set; }
            public double amount { get; set; }
            public bool isManualAdjustment { get; set; }
            public bool isPromissoryNote { get; set; }
            public bool canRefund { get; set; }
            public string externalPaymentTransactionData { get; set; }
            public string billingName { get; set; }
            public string referenceNumber { get; set; }
            public string notes { get; set; }
            public string method { get; set; }
            public string details { get; set; }
            public string createdIn { get; set; }
            public int id { get; set; }
            public DateTime createdAt { get; set; }
            public DateTime? updatedAt { get; set; }
        }

        public class UserProfileField
        {
            public string fieldId { get; set; }
            public string name { get; set; }
            public string value { get; set; }
        }

        public class Submission
        {
            public DateTime? roundSubmissionEndDate;
            public string solicitationName;
            public int solicitationId { get; set; }
            public int submissionId { get; set; }
            public string applicationName { get; set; }
            public string applicationCode { get; set; }
            public string categoryCode { get; set; }
            public string category { get; set; }
            public string fullCategoryPath { get; set; }
            public int owningOrganizationId { get; set; }
            public string solicitationCode { get; set; }
        }

        public class Invoice
        {
            public string userData;
            public int invoiceId { get; set; }
            public DateTime invoiceDate { get; set; }
            public bool paid { get; set; }
            public List<BillingLineItem> billingLineItems { get; set; }
            public List<Payment> payments { get; set; }
            public List<Refund> refunds { get; set; }
            public double totalBilled { get; set; }
            public double totalPaid { get; set; }
            public double totalRefund { get; set; }
            public int userId { get; set; }
            public string email { get; set; }
            public string firstName { get; set; }
            public string lastName { get; set; }
            public List<UserProfileField> userProfileFields { get; set; }
            public List<Submission> submissions { get; set; }
        }

        public class Refund
        {
            public int accountingTransactionId { get; set; }
            public double amount { get; set; }
            public string notes { get; set; }
            public string details { get; set; }
            public int id { get; set; }
            public DateTime createdAt { get; set; }
            public DateTime? updatedAt { get; set; }
        }
        #endregion
    }
}
