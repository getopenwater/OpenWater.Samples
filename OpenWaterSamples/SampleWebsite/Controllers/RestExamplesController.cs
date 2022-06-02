using OpenWater.ApiClient;
using OpenWater.ApiClient.FieldValues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SampleWebsite.Controllers
{
    public class RestExamplesController : Controller
    {
        public const string OpenWaterDomain = "your-domain.secure-platform.com";
        public const string OpenWaterApiKey = "00000000-0000-0000-0000-000000000000";
        
        public ActionResult GetApplicationsFromLast24Hours()
        {
            
            var api = new OpenWaterApiClient(OpenWaterDomain, OpenWaterApiKey);
            var apps = api.GetApplications(lastModifiedSinceUtc: DateTime.UtcNow.AddHours(-24)).Items;                   
            return Json(apps, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ShowDetails()
        {

            var api = new OpenWaterApiClient(OpenWaterDomain, OpenWaterApiKey);
            var apps = api.GetApplications(lastModifiedSinceUtc: DateTime.UtcNow.AddHours(-24)).Items;

            if (apps.Count == 0)
                return Content("No Data");

            var app = apps.First();
            var applicationDetails = api.GetApplicationById(app.Id);
            return Json(applicationDetails, JsonRequestBehavior.AllowGet);     
        }


        public ActionResult UpdateApplication()
        {
            var api = new OpenWaterApiClient(OpenWaterDomain, OpenWaterApiKey);
            var apps = api.GetApplications(lastModifiedSinceUtc: DateTime.UtcNow.AddHours(-24)).Items;

            if (apps.Count == 0)
                return Content("No Data");

            var app = apps.First();

            var applicationDetails = api.GetApplicationById(app.Id);


            api.UpdateRoundSubmissionFormValues(app.Id, new OpenWater.ApiClient.Application.UpdateRoundSubmissionFormValuesRequest
            {
                
                RoundId = applicationDetails.RoundSubmissions.First().RoundId,
                FieldValues = new List<FieldValueModelBase>()
                    {
                        new ApplicationNameFieldValueModel("title", "Updated from API"),
                        //new TextFieldValueModel("alias", "value")                        
                    }
            },
            suppressFormValidation: true);


            return Content("Done");
        }
    }
}