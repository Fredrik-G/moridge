using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Moridge.BusinessLogic;
using Moridge.Models;

namespace Moridge.Controllers
{
    [Authorize(Roles = RolesHelper.AdminRole)]
    public class AdminController : Controller
    {
        public ActionResult DriverRegister(bool forceUpdate = false)
        {
            var drivers = System.Web.HttpContext.Current.Session["Drivers"] as List<ApplicationUser>;
            if (drivers == null || forceUpdate)
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
        public ActionResult DriverDetails(int? index = null)
        {
            if (index == null)
            {
                return RedirectToAction("DriverRegister", "Admin");
            }
            var drivers = System.Web.HttpContext.Current.Session["Drivers"] as List<ApplicationUser>;
            if (drivers == null)
            {
                drivers = new DatabaseHelper().FindAllUsersInRole(RolesHelper.DriverRoleId);
                System.Web.HttpContext.Current.Session["Drivers"] = drivers;
            }
            var driver = drivers[index.Value];
            return View(new DriverDetailsModel { Driver = driver, Index = index.Value });
        }

        //
        // POST: /Admin/DriverDetails
        [HttpPost]
        public ActionResult DriverDetails(DriverDetailsModel model)
        {
            //update user
            var updateResult = new DatabaseHelper().UpdateUser(model.Driver);
            return View(model);
        }

        //
        // GET: /Admin/DriverCreate
        [HttpGet]
        public ActionResult DriverCreate()
        {
            return View(new DriverDetailsModel { IsCreatingNew = true });
        }

        //
        // POST: /Admin/DriverCreate
        [HttpPost]
        public ActionResult DriverCreate(DriverDetailsModel model)
        {
            //create user
            //TODO hur göra med lösenord?
            var password = MyMoridgeServer.BusinessLogic.Common.GeneratePassword(model.FirstName.ToLower() + model.LastName.ToLower());

            var dbHelper = new DatabaseHelper();
            var creationResult = dbHelper.CreateUser(model.Driver, password);

            if (creationResult.Succeeded)
            {
                //assign role to user and give default schedule if driver
                switch (model.Roles)
                {
                    case RolesHelper.Roles.Driver:
                        RolesHelper.AddUserToRole(dbHelper, model.Driver.Id, RolesHelper.DriverRole);
                        var schedule = new Schedule(model.Driver.Id, dbHelper);
                        schedule.CreateDefaultSchedule();
                        break;
                    case RolesHelper.Roles.Admin:
                        RolesHelper.AddUserToRole(dbHelper, model.Driver.Id, RolesHelper.AdminRole);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return RedirectToAction("DriverRegister", "Admin", new { forceUpdate = true });
            }

            //errors => re-show page.
            return View(model);
        }

        public ActionResult DriverDelete(string userId, string index)
        {
            var result = new DatabaseHelper().DeleteUser(userId);
            if (result.Succeeded)
            {
                return RedirectToAction("DriverRegister", "Admin", new { forceUpdate = true });
            }

            //errors => re-show page.
            return RedirectToAction("DriverDetails", "Admin", new { index = index });
        }
    }
}
