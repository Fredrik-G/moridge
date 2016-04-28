using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Google.Apis.Calendar.v3.Data;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Moridge.BusinessLogic;
using Moridge.Extensions;
using Moridge.Models;
using MyMoridgeServer.BusinessLogic;

namespace Moridge.Controllers
{
    [Authorize(Roles = RolesHelper.DRIVER_ROLE + "," + RolesHelper.ADMIN_ROLE)]
    public class DriverController : Controller
    {
        private readonly Schedule _schedule = new Schedule();

        public ActionResult Booking()
        {
            var calendar = new GoogleCalendar(Common.GetAppConfigValue("MoridgeOrganizerCalendarEmail"), Common.GetAppConfigValue("MoridgeMainCalendarEmail"));
            var events = calendar.GetEventList();
            System.Web.HttpContext.Current.Session["AllEvents"] = events.Items;

            return View(new BookingModel(events.Items));
        }

        public void SetClickedDate(string date, string occassion)
        {
            //var bookingModel = new BookingModel { Events = System.Web.HttpContext.Current.Session["AllEvents"] as IList<Event> };
            //System.Web.HttpContext.Current.Session["Events"] = bookingModel.GetBookingsForOccasion(date, occassion);
        }

        public ActionResult BookingDetails(string date, string occassion)
        {
            var events = System.Web.HttpContext.Current.Session["AllEvents"] as IList<Event>;
            var bookingModel = new BookingModel(events);
            bookingModel.Booking.GetBookingsForOccasion(date, occassion);
            return View(bookingModel);
        }

        public ActionResult _PopupPartial()
        {
            return PartialView();
        }

        [HttpGet]
        public ActionResult Schedule(bool useLocalValues = false)
        {
            var schedule = useLocalValues ? System.Web.HttpContext.Current.Session["Schedule"] as List<ScheduleModel>
                                          : _schedule.GetDriverSchedule();
            return View(schedule);
        }

        [HttpPost]
        public ActionResult Schedule(IEnumerable<ScheduleModel> schedule)
        {
            System.Web.HttpContext.Current.Session["Schedule"] = _schedule.SaveDriverSchedule(schedule);
            return RedirectToAction("Schedule", "Driver", new { useLocalValues = true});
        }

        public ActionResult PersonalInfo()
        {
            return View();
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
