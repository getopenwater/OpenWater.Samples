using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

using SampleFunctions.Models;
using SampleFunctions.Extensions;
using System.Net.Http.Headers;
using JWT;
using System.Collections.Generic;

namespace SampleFunctions
{
    public static class SingleSignOn
    {
        public const string OpenWaterHost = "sandbox-course.secure-platform.com";
        public const string JwtSecret = "jwt-secret";

        public static string DefaultOpenWaterReturnUrl = $"https://{OpenWaterHost}/a/account/validatethirdpartycorporateauthresult?redirectUrl={System.Uri.EscapeDataString("https://" + OpenWaterHost + "/a")}";


        [FunctionName("Begin")]
        public static async Task<HttpResponseMessage> Begin([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "sso/begin")]HttpRequestMessage req, TraceWriter log)
        {
            //Try and get the return URL from OpenWater Initialized, if none is set, use the default
            var returnUrl = DefaultOpenWaterReturnUrl;
            try
            {
                returnUrl = req.GetQueryNameValuePairs().FirstOrDefault(c => string.Compare(c.Key, "returnUrl") == 0).Value;
                if (returnUrl == null)
                    returnUrl = DefaultOpenWaterReturnUrl;
            }
            catch
            {
                returnUrl = DefaultOpenWaterReturnUrl;
            }


            var processUrl = req.RequestUri.AbsoluteUri.ToString().Replace("begin", "process").Split('?')[0];
            var redirectUrl = $"http://localhost:49962/login?callback={System.Uri.EscapeUriString(processUrl)}";

            var response = req.Redirect(redirectUrl);
            var cookie = new CookieHeaderValue("returnUrl", returnUrl);
            response.Headers.AddCookies(new CookieHeaderValue[] { cookie });

            return response;
        }

        [FunctionName("Process")]
        public static async Task<HttpResponseMessage> Process([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "sso/process")]HttpRequestMessage req, TraceWriter log)
        {
            //Try and get the return URL from saved in begin step from cookie, if none is set, use the default
            var returnUrl = DefaultOpenWaterReturnUrl;
            try
            {
                returnUrl = req.Headers.GetCookies().First().Cookies.FirstOrDefault(c => c.Name == "returnUrl").Value;
                if (returnUrl == null)
                    returnUrl = DefaultOpenWaterReturnUrl;
            }
            catch
            {
                returnUrl = DefaultOpenWaterReturnUrl;
            }

            var token = req.GetQueryNameValuePairs().Where(c => c.Key.ToLower() == "token").First().Value;

            var http = new HttpClient();
            var headers = new Dictionary<string, string>();
            headers.Add("Authorization", token);

            var getUserDetails = http.Get("http://localhost:49962/GetUserInfo", headers).ReadContentAs<DemoUserData>();

            var profileTextFieldData = new Dictionary<string, string>();
            profileTextFieldData.Add("946adfc5-f8a4-43e5-a044-59b08c04e002", getUserDetails.FirstName + " " + getUserDetails.LastName);

            var connectorResult = new JwtResultWithTimestamp
            {
                FirstName = getUserDetails.FirstName,
                LastName = getUserDetails.LastName,
                Email = getUserDetails.EmailAddress,                
                UserData = getUserDetails.Id.ToString(),
                ThirdPartyUniqueId = getUserDetails.Id.ToString(),
                Company = "abc",
                ProfileTextFieldData = profileTextFieldData
            };

            var jwt = JsonWebToken.Encode(connectorResult, JwtSecret, JwtHashAlgorithm.HS256);
            var response = req.Redirect($"{returnUrl}&token={jwt}");
            return response;
        }

        public class DemoUserData
        {
            public int Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string EmailAddress { get; set; }
            public string Password { get; set; }
            public string FullName { get; set; }
        }
    }
}
