using System.Runtime.CompilerServices;
using System.Web.Mvc;
using System.Web.Routing;

namespace Diary.Controllers
{
    public class SecurityController : Controller
    {
        private static RouteValueDictionary ReturnToRoute = null;

        private static readonly object AUTH = true;
        public static void CheckAuth(Controller controller, [CallerMemberName] string action = "")
        {
            if (controller.TempData.Peek("IsAuthorised") != AUTH)
            {
                ReturnToRoute = controller.RouteData.Values;
                
                controller.Response.Redirect("/Security/Login");
            }
        }

        // GET: Security/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST Security/Login
        [HttpPost]
        public ActionResult Login(string password)
        {
            if (password != "Ph0enix0")
            {
                ViewBag.Error = "true";
                return View();
            }

            TempData["IsAuthorised"] = AUTH;
            return RedirectToRoute(ReturnToRoute);
        }
    }
}