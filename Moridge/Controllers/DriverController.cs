using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Google.Apis.Calendar.v3.Data;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Moridge.BusinessLogic;
using Moridge.Models;
using MyMoridgeServer.BusinessLogic;

namespace Moridge.Controllers
{
    [Authorize(Roles = RolesHelper.DRIVER_ROLE + "," + RolesHelper.ADMIN_ROLE)]
    public class DriverController : Controller
    {
        public ActionResult Booking()
        {
            var context = new ApplicationDbContext();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var user = userManager.FindByEmail("fredrikgummus@gmail.com");

            var monday = DayOfWeek.Monday.ToString();
            var mondaySchedule = user.Schedule.SingleOrDefault(x => x.DayOfWeek == monday);
            var deviation = user.ScheduleDeviation.First();

            //ICollection<ScheduleDeviation> devations = new[] {
            //    new ScheduleDeviation
            //    {
            //        Afternoon = 1,
            //        Morning = 2,
            //        Date = DateTime.Now.AddDays(2)
            //    },
            //    new ScheduleDeviation
            //    {
            //        Afternoon = 3,
            //        Morning = 4,
            //        Date = DateTime.Now.AddDays(3)
            //    }
            //};
            //user.ScheduleDeviation = devations;
            //var asd = new DaySchedule
            //{
            //    Id = 1,
            //    DayOfWeek = "Monday",
            //    Morning = 4,
            //    Afternoon = 5,
            //};
            //user.Schedule.Add(asd);
            //context.SaveChanges();
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

        public ActionResult Schedule()
        {
            return View(new ScheduleModel());
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
