using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Moridge.BusinessLogic;
using Moridge.Models;

namespace Moridge.Controllers
{
    [Authorize(Roles = RolesHelper.ADMIN_ROLE)]
    public class AdminController : Controller
    {
        public ActionResult DriverRegister()
        {
            var dbHandler = new DatabaseHelper();
            var drivers = dbHandler.FindAllUsersInRole(RolesHelper.DRIVER_ROLE);
            var model = new DriverRegisterModel { Drivers = drivers };
            return View(model);
        }
    }
}
