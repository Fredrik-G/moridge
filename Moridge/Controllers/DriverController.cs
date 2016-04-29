using System.Collections.Generic;
using System.Web.Mvc;
using Google.Apis.Calendar.v3.Data;
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
            var schedule = new Schedule();
            var scheduleSet = new ScheduleModelSet();
            var driverSchedule = useLocalValues ? System.Web.HttpContext.Current.Session["Schedule"] as List<ScheduleModel>
                                                : schedule.GetDriverSchedule();
            scheduleSet.ScheduleModels = driverSchedule;
            System.Web.HttpContext.Current.Session["Schedule"] = driverSchedule;
            return View(scheduleSet);
        }

        [HttpPost]
        public ActionResult Schedule(ScheduleModelSet scheduleSet)
        {
            var schedule = new Schedule();
            System.Web.HttpContext.Current.Session["Schedule"] = schedule.SaveDriverSchedule(scheduleSet.ScheduleModels);
            return RedirectToAction("Schedule", "Driver", new { useLocalValues = true });
        }

        public ActionResult ScheduleDeviation()
        {
            var driverSchedule = System.Web.HttpContext.Current.Session["Schedule"] as List<ScheduleModel>;
            if (driverSchedule == null)
            {
                var schedule = new Schedule();
                driverSchedule = schedule.GetDriverSchedule();
            }

            var scheduleSet = new ScheduleModelSet { ScheduleModels = driverSchedule };
            scheduleSet.SetCurrentWeek();
            return View(scheduleSet);
        }

        public ActionResult PersonalInfo()
        {
            return View();
        }
    }
}
