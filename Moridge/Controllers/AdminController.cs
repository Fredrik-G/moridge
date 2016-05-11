using System.Collections.Generic;
using System.Web.Mvc;
using Moridge.BusinessLogic;
using Moridge.Models;

namespace Moridge.Controllers
{
    [Authorize(Roles = RolesHelper.AdminRole)]
    public class AdminController : Controller
    {
        public ActionResult DriverRegister()
        {
            var dbHandler = new DatabaseHelper();
            var drivers = System.Web.HttpContext.Current.Session["Drivers"] as List<ApplicationUser>;
            if (drivers == null)
            {
                drivers = dbHandler.FindAllUsersInRole(RolesHelper.DriverRoleId);
                System.Web.HttpContext.Current.Session["Drivers"] = drivers;
            }
            var model = new DriverRegisterModel { Drivers = drivers };
            return View(model);
        }
    }
}
