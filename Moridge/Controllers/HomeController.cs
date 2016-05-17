using System.Web.Mvc;
using Moridge.BusinessLogic;

namespace Moridge.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            //Gets the next page based off the user's role.
            string actionName, controllerName;
            if (Request.IsAuthenticated)
            {
                RolesHelper.GetPageForUser(HttpContext.User, out actionName, out controllerName);
                UserHelper.SaveUserToSession();
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
