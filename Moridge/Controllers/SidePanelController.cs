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
            var model = new SidePanelModel();
            return PartialView(model);
        }

        [AllowAnonymous]
        [ChildActionOnly]
        public ActionResult _SidePanelDefault()
        {
            var model = new SidePanelModel();
            return PartialView(model);
        }
    }
}
