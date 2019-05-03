using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.IO;

namespace SampleFunctions.Extensions
{
    public static class HttpRequestMessageExtensions
    {
        public static HttpResponseMessage Redirect(this HttpRequestMessage self, string url)
        {
            var response = new HttpResponseMessage(HttpStatusCode.Redirect);
            response.Headers.Location = new Uri(url);
            return response;
        }

        public static HttpResponseMessage Html(this HttpRequestMessage self, string content)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(content);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }

        public static HttpResponseMessage Text(this HttpRequestMessage self, string content, string mediaType = "text/plain")
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(content);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(mediaType);
            return response;
        }

        public static HttpResponseMessage DownloadText(this HttpRequestMessage self, string content, string filename)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(content);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = filename };
            return response;
        }

        public static HttpResponseMessage DownloadFile(this HttpRequestMessage self, byte[] content, string filename)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new ByteArrayContent(content);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = filename };
            return response;
        }

        public static HttpResponseMessage DownloadZip(this HttpRequestMessage self, byte[] content, string folderName)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new ByteArrayContent(content);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/zip");
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = folderName };
            return response;
        }

        public static HttpResponseMessage Json(this HttpRequestMessage self, object data, bool prettyPrint = false)
        {
            if (data == null)
                data = new object();

            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(data.ToJson(prettyPrint), Encoding.UTF8, "application/json");
            response.Content.Headers.ContentType.CharSet = Encoding.UTF8.HeaderName;
            return response;
        }

        public static HttpResponseMessage Javascript(this HttpRequestMessage self, string data)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(data, Encoding.UTF8);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/javascript");
            response.Content.Headers.ContentType.CharSet = Encoding.UTF8.HeaderName;
            return response;
        }

        public static HttpResponseMessage Css(this HttpRequestMessage self, string data)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(data);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/css");
            response.Content.Headers.ContentType.CharSet = Encoding.UTF8.HeaderName;
            return response;
        }

        public static void DisableCache(this HttpRequestMessage self)
        {
            self.Headers.CacheControl = new CacheControlHeaderValue()
            {
                NoCache = true,
                NoStore = true,
                MaxAge = new TimeSpan(0),
                MustRevalidate = true,
            };

            self.Headers.Add("pragma", "no-cache");
        }

        public static void EnableCors(this HttpResponseMessage self)
        {
            self.Headers.Add("Access-Control-Allow-Origin", "*");
        }

        public static void EnableCors(this HttpResponseMessage self, string domain)
        {
            self.Headers.Add("Access-Control-Allow-Origin", domain);
        }

        public static void EnableCorsAllowCredentials(this HttpResponseMessage self)
        {
            self.Headers.Add("Access-Control-Allow-Credentials", "true");
        }
    }
}
