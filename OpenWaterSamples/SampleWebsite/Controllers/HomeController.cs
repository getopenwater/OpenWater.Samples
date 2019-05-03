using SampleWebsite.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SampleWebsite.Controllers
{
    public class HomeController : Controller
    {
        private const string ERROR_MESSAGE_KEY = "ErrorMessage";
        public ActionResult Index()
        {
            return View();
        }

        [Route("login")]
        public ActionResult Login(string callback)
        {
            if (TempData[ERROR_MESSAGE_KEY] != null)
            {
                ViewData[ERROR_MESSAGE_KEY] = TempData[ERROR_MESSAGE_KEY];
            }
            return View();
        }

        [HttpPost]
        [Route("login")]
        public ActionResult Login(string username, string password, string callback)
        {
            var user = InMemoryDatabase.GetUserByEmailAndPassword(username, password);
            if (user == null)
            {
                TempData[ERROR_MESSAGE_KEY] = "Invalid username or password";
                return RedirectToAction("login", new { callback = callback });
            }
            else
            {
                if (string.IsNullOrEmpty(callback))
                {
                    return RedirectToAction("index");
                }
                UserTokensService tokensService = new UserTokensService();
                var token = tokensService.AddNewUserToken(user);
                var redirectUrl = $"{callback}?token={token}";
                Response.Redirect(redirectUrl);
            }
            return RedirectToAction("index");
        }
    }
}