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
            var drivers = System.Web.HttpContext.Current.Session["Drivers"] as List<ApplicationUser>;
            if (drivers == null)
            {
                drivers = new DatabaseHelper().FindAllUsersInRole(RolesHelper.DriverRoleId);
                System.Web.HttpContext.Current.Session["Drivers"] = drivers;
            }
            var model = new DriverRegisterModel { Drivers = drivers };
            return View(model);
        }

        //
        // GET: /Admin/DriverDetails
        [HttpGet]
        public ActionResult DriverDetails(int index)
        {
            var drivers = System.Web.HttpContext.Current.Session["Drivers"] as List<ApplicationUser>;
            if (drivers == null)
            {
                drivers = new DatabaseHelper().FindAllUsersInRole(RolesHelper.DriverRoleId);
                System.Web.HttpContext.Current.Session["Drivers"] = drivers;
            }
            var driver = drivers[index];
            return View(new DriverDetailsModel { Driver = driver });
        }

        //
        // POST: /Admin/DriverDetails
        [HttpPost]
        public ActionResult DriverDetails(DriverDetailsModel model)
        {
            return View(model);
        }

        //
        // GET: /Admin/DriverCreate
        [HttpGet]
        public ActionResult DriverCreate()
        {
            return View();
        }

        //
        // POST: /Admin/DriverCreate
        [HttpPost]
        public ActionResult DriverCreate(DriverDetailsModel model)
        {
            //create user
            return RedirectToAction("DriverRegister", "Admin");
        }
    }
}
