using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SampleWebsite.Controllers
{
    public class LookupController : Controller
    {
        // GET: Lookup
        public ActionResult Search(string query)
        {
            if (query == null)
                query = "";

            var results = FakeDatabase().Where(c => c.FullName.ToLower().Contains(query.ToLower())).Select(c => new AutoCompleteItem
            {
                label = c.FullName,
                value = c.Id
            }).ToArray();

            Response.Headers.Add("Access-Control-Allow-Origin", "*");
            return Json(results, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetItem(string value)
        {
            var result = FakeDatabase().Where(c => c.Id == value).First();


            var prefillItemList = new List<PrefillItem>();
            
            //First Field to Prefill
            var prefillItem = new PrefillItem
            {
                tableAlias = "",
                fields = new List<PrefillItemField>()
            };
            prefillItem.fields.Add(new PrefillItemField { alias = "textFieldsCanWordCounters", value = $"{result.FirstName} {result.LastName} - {result.University}" });
            prefillItem.fields.Add(new PrefillItemField { alias = "enterANumber", value = Convert.ToInt32(result.Id) });
            prefillItemList.Add(prefillItem);
           
            //Assemble JSON Response
            Response.Headers.Add("Access-Control-Allow-Origin", "*");
            return Json(prefillItemList, JsonRequestBehavior.AllowGet);
        }

        public class AutoCompleteItem
        {
            public string label { get; set; }
            public string value { get; set; }
        }

        public class PrefillItem
        {
            public string tableAlias { get; set; }
            public List<PrefillItemField> fields { get; set; }
        }

        public class PrefillItemField
        {
            public string alias { get; set; }
            public object value { get; set; }
        }



        private static List<LookupTestItem> FakeDatabase()
        {
            var db = new List<LookupTestItem>();

            db.Add(new LookupTestItem { Id = "1", FirstName = "John", LastName = "Doe", University = "Florida State University" });
            db.Add(new LookupTestItem { Id = "2", FirstName = "Jane", LastName = "Doe", University = "Ohio State University" });
            db.Add(new LookupTestItem { Id = "3", FirstName = "Bill", LastName = "Gates", University = "Harvard" });
            db.Add(new LookupTestItem { Id = "4", FirstName = "Mark", LastName = "Zuckerberg", University = "Harvard" });
            db.Add(new LookupTestItem { Id = "5", FirstName = "Larry", LastName = "Ellison", University = "University of California, Berkeley" });
            db.Add(new LookupTestItem { Id = "6", FirstName = "Michelle", LastName = "Obama", University = "Princeton" });
            db.Add(new LookupTestItem { Id = "7", FirstName = "Oprah", LastName = "Winfrey", University = "None" });

            return db;
        }

        public class LookupTestItem
        {
            public string Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string University { get; set; }

            public string FullName => FirstName + " " + LastName;
        }
    }
}