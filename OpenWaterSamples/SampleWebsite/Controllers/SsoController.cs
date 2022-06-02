using JWT;
using SampleWebsite.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SampleWebsite.Controllers
{
    public class SsoController : Controller
    {
        //TODO: Set these to match your instance
        public const string OpenWaterHost = "your-domain.secure-platform.com";
        public const string JwtSecret = "your-secret";

        public static string DefaultOpenWaterReturnUrl = $"https://{OpenWaterHost}/a/account/validatethirdpartycorporateauthresult?redirectUrl={System.Uri.EscapeDataString("https://" + OpenWaterHost + "/a")}";

        // GET: Sso
        [AllowAnonymous]
        public ActionResult Begin(string returnUrl)
        {
            //This is where we return to after logging in
            if(!string.IsNullOrEmpty(returnUrl))
                Response.Cookies.Add(new HttpCookie("returnUrl", returnUrl)); //Deep Link in OpenWater to Return to

            //SSO Process Hnalder
            var processUrl = Request.Url.AbsoluteUri.ToString().Replace("begin", "process").Split('?')[0];

            
            var redirectUrl = $"/login?callback={System.Uri.EscapeUriString(processUrl)}";            
            return Redirect(redirectUrl);
        }

        public ActionResult Process(string token)
        {
            var returnUrl = DefaultOpenWaterReturnUrl;
            if(Request.Cookies.AllKeys.Contains("returnUrl"))
            {
                //This is where we return to after logging in
                if (!string.IsNullOrEmpty(Request.Cookies["returnUrl"].Value))
                    returnUrl = Request.Cookies["returnUrl"].Value;
            }
                        
            //Get Logged in User from Data Store
            UserTokensService tokensService = new UserTokensService();
            var user = tokensService.GetUserByToken(token);

            var connectorResult = new JwtResultWithTimestamp
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.EmailAddress,
                UserData = user.Id.ToString(),
                ThirdPartyUniqueId = user.Id.ToString(),
                Company = "abc"
            };

            var jwt = JsonWebToken.Encode(connectorResult, JwtSecret, JwtHashAlgorithm.HS256);
            var finalRedirectUrl = ($"{returnUrl}&token={jwt}");            
            return Redirect(finalRedirectUrl);
        }


        public class JwtResultWithTimestamp
        {
            public bool UserNameExists { get; set; }
            public bool UserValidatedSuccessfully { get; set; }
            public string SSOToken { get; set; }
            public DateTime TimestampUtc => DateTime.UtcNow;
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }

            public string UserData { get; set; }
            public bool UserIsMember { get; set; }

            public string Company { get; set; }
            public string JobTitle { get; set; }
            public Address PrimaryAddress { get; set; }
            public Address SecondaryAddress { get; set; }
            public string Phone { get; set; }
            public string Website { get; set; }

            public Dictionary<string, string> ProfileTextFieldData { get; set; }
            public string ThirdPartyUniqueId { get; set; }

            public static readonly Guid FirstNameFieldId = new Guid("c88b266c-b8bb-4da8-94dd-aedc0bf3bd42");
            public static readonly Guid LastNameFieldId = new Guid("d667d690-06c4-4867-a431-839be6e48458");

            public static readonly Guid CompanyFieldId = new Guid("f2e55e0d-a7a7-4748-b42c-ef3d45a78e96");
            public static readonly Guid SalutationFieldId = new Guid("8e7af69d-d445-4faa-adad-e3e4fc17fc5a");
            public static readonly Guid SuffixFieldId = new Guid("e63614c7-463a-4e47-9714-9db9d2af2062");
            public static readonly Guid PrimaryAddressFieldId = new Guid("5b9e40ec-678e-4bfa-bba3-867b00e0df4d");
            public static readonly Guid SecondaryAddressFieldId = new Guid("46472988-d4a1-4fd6-9b80-1efb1e5c762c");
            public static readonly Guid JobTitleFieldId = new Guid("f139e8f2-3755-4804-81b7-6c363022bc08");
            public static readonly Guid WebsiteFieldId = new Guid("b9d98955-7ed9-473b-bcd1-adc693ef9f5b");
            public static readonly Guid PhoneNumberFieldId = new Guid("4ac60a65-012a-4a92-83ef-7c98e35e11de");
            public static readonly Guid FaxNumberFieldId = new Guid("ab492fad-b006-460e-b195-27f970fbf852");
            public static readonly Guid BioFieldId = new Guid("1f1b93e7-fd6a-49c7-b9c6-2b3b6bb21d9e");
            public static readonly Guid PhotoFieldId = new Guid("f66863bc-3268-4bc1-bd6e-77b182200f31");
            public static readonly Guid GenderFieldId = new Guid("ab2f5484-9c78-4944-8377-473f4f4cc369");
            public static readonly Guid BirthdayFieldId = new Guid("150d17d5-6ee9-4510-9ef1-9e5635b0c241");
        }


        public class Address
        {
            public string Line1 { get; set; }
            public string Line2 { get; set; }
            public string City { get; set; }
            public string CountryAbbreviationOrName { get; set; }
            public string StateProvinceAbbreviationOrName { get; set; }
            public string ZipPostalCode { get; set; }

        }


    }
}