using System.Web.Mvc;
using Moridge.BusinessLogic;
using Moridge.Models;

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
            if (Request.IsAuthenticated)
            {
                RolesHelper.GetPageForUser(HttpContext.User, out actionName, out controllerName);
                SidePanelHelper.SaveUserToSession();
            }
            else
            {
                actionName = "Login";
                controllerName = "Account";
            }
            return RedirectToAction(actionName, controllerName);
        }
    }
}
