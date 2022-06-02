using SampleWebsite.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SampleWebsite.Controllers
{
    /// <summary>
    /// This is a sample login page.  There is no need to update this site as it simulates a login page.
    /// </summary>
    public class HomeController : Controller
    {
        private const string ERROR_MESSAGE_KEY = "ErrorMessage";
        private const string USER_COOKIE_KEY = "_User";
        public ActionResult Index()
        {
            var user = GetLoggedInUser();
            if (user != null)
            {
                return View(user);
            }

            return RedirectToAction("login");
        }

        [Route("login")]
        public ActionResult Login(string callback)
        {
            var user = GetLoggedInUser();
            if (user != null)
            {
                if (!string.IsNullOrEmpty(callback))
                {
                    var redirectUrl = GenerateTokenAndRedirect(callback, user);
                    return Redirect(redirectUrl);
                }
                else
                {
                    return RedirectToAction("index");
                }
            }
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
                SignInUser(user);
                if (string.IsNullOrEmpty(callback))
                {
                    return RedirectToAction("index");
                }
                var redirectUrl = GenerateTokenAndRedirect(callback, user);
                return Redirect(redirectUrl);
            }
        }

        private string GenerateTokenAndRedirect(string callback, User user)
        {
            UserTokensService tokensService = new UserTokensService();
            var token = tokensService.AddNewUserToken(user);

            if(callback.Contains("?"))
            {
                var redirectUrl = $"{callback}&token={token}";
                return redirectUrl;
            }
            else
            {
                var redirectUrl = $"{callback}?token={token}";
                return redirectUrl;
            }
            
        }

        [Route("GetUserInfo")]
        public ActionResult GetUserInfo()
        {
            var token = Request.Headers["Authorization"];
            if (token == null)
            {
                return Json(new { Message = "Please provide authenticaiton token" }, JsonRequestBehavior.AllowGet);
            }
            UserTokensService tokensService = new UserTokensService();
            var user = tokensService.GetUserByToken(token);
            if (user == null)
            {
                return Json(new { Message = "Invalid Token" }, JsonRequestBehavior.AllowGet);
            }
            return Json(user, JsonRequestBehavior.AllowGet);
        }

        private void SignInUser(User user)
        {
            HttpCookie cookie = new HttpCookie(USER_COOKIE_KEY, user.Id.ToString());
            Response.Cookies.Add(cookie);
        }

        private User GetLoggedInUser()
        {
            if (Request.Cookies[USER_COOKIE_KEY] == null)
            {
                return null;
            }

            var userId = Request.Cookies[USER_COOKIE_KEY].Value;
            return InMemoryDatabase.GetUserById(int.Parse(userId));
        }

        [Route("logout")]
        public ActionResult Logout()
        {
            Response.Cookies[USER_COOKIE_KEY].Expires = DateTime.Now.AddDays(-1);
            return RedirectToAction("login");
        }

    }
}