using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Moridge.BusinessLogic;
using Moridge.Models;
using MyMoridgeServer.BusinessLogic;

namespace Moridge.Controllers
{
    [Authorize(Roles = RolesHelper.AdminRole)]
    public class AdminController : Controller
    {
        #region Driver Related

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
            var password =
                Common.GeneratePassword(model.FirstName.ToLower() +
                                                                      model.LastName.ToLower());

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

        #endregion

        #region Statistics

        public ActionResult StatisticsCompanies()
        {
            int bookingCount;
            var model = new Statistics().SetupModels(out bookingCount);
            System.Web.HttpContext.Current.Session["CompanyModels"] = model.StatisticsModels;
            System.Web.HttpContext.Current.Session["BookingCount"] = bookingCount;
            return View(model);
        }

        public ActionResult StatisticsDrivers()
        {
            int bookingCount;
            var model = new Statistics().SetupModels(out bookingCount, true);
            System.Web.HttpContext.Current.Session["DriverModels"] = model.StatisticsModels;
            System.Web.HttpContext.Current.Session["BookingCount"] = bookingCount;

            return View(model);
        }

        public ActionResult StatisticsDetails(int index, bool isForDrivers = false)
        {
            var models = System.Web.HttpContext.Current.Session[
                isForDrivers ? "DriverModels" : "CompanyModels"] as List<StatisticsModel>;
            if (models == null)
            {
                int bookingCount;
                models = new Statistics().SetupModels(out bookingCount, isForDrivers).StatisticsModels;
                System.Web.HttpContext.Current.Session[isForDrivers ? "DriverModels" : "CompanyModels"] = models;
            }

            var model = models[index];
            model.Index = index;
            return View(model);
        }

        public ActionResult StatisticsChart(int? index, bool showTotal = false, bool useDates = true, bool isForDrivers = false)
        {
            var models = System.Web.HttpContext.Current.Session[isForDrivers ? "DriverModels" : "CompanyModels"] as List<StatisticsModel>;
            var chartModel = new Statistics().SetupChart(showTotal ? null : models[index.Value],
                showTotal ? models : null, useDates);
            return View(chartModel);
        }

        public ActionResult StatisticsTotal(bool isForDrivers)
        {
            //todo session för förare
            var models = System.Web.HttpContext.Current.Session[isForDrivers ? "DriverModels" : "CompanyModels"] as List<StatisticsModel>;
            var bookingCount = System.Web.HttpContext.Current.Session["BookingCount"] is int
                ? (int)System.Web.HttpContext.Current.Session["BookingCount"]
                : 0;
            if (models == null)
            {
                models = new Statistics().SetupModels(out bookingCount, isForDrivers).StatisticsModels;
                System.Web.HttpContext.Current.Session[isForDrivers ? "DriverModels" : "CompanyModels"] = models;
            }
            return View(new StatisticsSetModel { StatisticsModels = models, BookingCount = bookingCount, IsForDriver = isForDrivers });
        }

        #endregion
    }
}
