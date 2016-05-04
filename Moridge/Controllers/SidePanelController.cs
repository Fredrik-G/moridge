using System.Web.Mvc;
using Moridge.BusinessLogic;
using Moridge.Models;

namespace Moridge.Controllers
{
    [Authorize]
    public class SidePanelController : Controller
    {
        [ChildActionOnly]
        public ActionResult _SidePanelLoggedIn()
        {
            var user = System.Web.HttpContext.Current.Session["SidePanelUser"] as SidePanelUser;
            var model = new SidePanelLoggedInModel { User = user };
            return PartialView(model);
        }

        [AllowAnonymous]
        [ChildActionOnly]
        public ActionResult _SidePanelDefault()
        {
            return PartialView();
        }
    }
}
