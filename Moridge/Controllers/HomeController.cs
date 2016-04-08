using System.Web.Mvc;
using Moridge.Helpers;

namespace Moridge.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            //Gets the next page based off the user's role.
            string actionName, controllerName;
            RolesHelper.GetPageForUser(HttpContext.User, out actionName, out controllerName);

            return RedirectToAction(actionName, controllerName);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
