using System.Web.Mvc;
using Moridge.BusinessLogic;

namespace Moridge.Controllers
{
    public class FloatingActionButtonController : Controller
    {
        [ChildActionOnly]
        public ActionResult _FloatingActionButton()
        {
            var user = UserHelper.GetCurrentUser();
            return user.Role.Equals(RolesHelper.DriverRole) ? PartialView() : null;
        }
    }
}
