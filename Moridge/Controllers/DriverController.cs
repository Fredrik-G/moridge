using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Google.Apis.Calendar.v3.Data;
using Moridge.BusinessLogic;
using Moridge.Extensions;
using Moridge.Models;
using MyMoridgeServer.BusinessLogic;
using Booking = Moridge.BusinessLogic.Booking;

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

        public ActionResult BookingDay(string date = null)
        {
            var calendar = new GoogleCalendar(Common.GetAppConfigValue("MoridgeOrganizerCalendarEmail"), Common.GetAppConfigValue("MoridgeMainCalendarEmail"));
            var events = calendar.GetEventList();
            System.Web.HttpContext.Current.Session["AllEvents"] = events.Items;

            //setup date
            var today = Day.GetSwedishTime(DateTime.Now).ToString("yyyy-M-d");
            date = date ?? today;
            var isToday = date.Equals(today);
            var dateTime = Day.ConvertStringToDateTime(date);

            var bookingModel = new BookingDayModel(events.Items, date) { DateTime = dateTime, IsToday = isToday };
            bookingModel.Booking.GetBookingsForOccasion(date, "Förmiddag");
            bookingModel.Booking.GetBookingsForOccasion(date, "Eftermiddag");
            return View(bookingModel);
        }

        //
        // GET: /Driver/BookingCreate
        [HttpGet]
        public ActionResult BookingCreate(string parentDate)
        {
            var calendar = new GoogleCalendar(Common.GetAppConfigValue("MoridgeOrganizerCalendarEmail"), Common.GetAppConfigValue("MoridgeMainCalendarEmail"));
            var events = calendar.GetEventList();
            System.Web.HttpContext.Current.Session["AllEvents"] = events.Items;

            return View(new BookingCreateModel { ParentDate = parentDate });
        }

        //
        // POST: /Driver/BookingCreate
        [HttpPost]
        public ActionResult BookingCreate(BookingCreateModel model)
        {
            if (!ModelState.IsValid) return View(model);
            //do stuff

            var booking = new Booking();
            booking.BookEvent(model);
            return RedirectToAction("BookingDay", "Driver", new { date = model.ParentDate});
        }

        public ActionResult BookingWeek()
        {
            var calendar = new GoogleCalendar(Common.GetAppConfigValue("MoridgeOrganizerCalendarEmail"), Common.GetAppConfigValue("MoridgeMainCalendarEmail"));
            var events = calendar.GetEventList();
            System.Web.HttpContext.Current.Session["AllEvents"] = events.Items;

            return View(new BookingModel(events.Items) { Date = Day.GetSwedishTime(DateTime.Now) });
        }

        public ActionResult BookingEvent(string eventId, BookingEventModel.EventStatus eventStatus = BookingEventModel.EventStatus.NotSet)
        {
            var calendar = new GoogleCalendar(Common.GetAppConfigValue("MoridgeOrganizerCalendarEmail"), Common.GetAppConfigValue("MoridgeMainCalendarEmail"));
            var bookingEvent = calendar.GetEvent(eventId);
            return View(new BookingEventModel { Event = bookingEvent, CurrentStatus = eventStatus });
        }

        public ActionResult BookingEventUpdate(BookingEventModel model)
        {
            return RedirectToAction("BookingEvent", "Driver", new { eventId = model.Event.Id, eventStatus = model.CurrentStatus });
        }

        public ActionResult BookingDetails(string date, string occassion, int bookingsForDriver)
        {
            var events = System.Web.HttpContext.Current.Session["AllEvents"] as IList<Event>;
            var bookingModel = new BookingModel(events) { BookingsForDriver = bookingsForDriver };
            bookingModel.Booking.GetBookingsForOccasion(date, occassion);
            return View(bookingModel);
        }

        public ActionResult _PopupPartial()
        {
            return PartialView();
        }

        //
        // GET: /Driver/Schedule
        [HttpGet]
        public ActionResult Schedule(bool useLocalValues = false)
        {
            var driverSchedule = useLocalValues
                ? System.Web.HttpContext.Current.Session["Schedule"] as List<ScheduleModel>
                : new Schedule().GetDriverSchedule();

            var scheduleSet = new ScheduleModelSet
            {
                IsDeviationSet = false,
                WeeksFromNow = 0,
                ScheduleModels = driverSchedule
            };
            System.Web.HttpContext.Current.Session["Schedule"] = driverSchedule;
            return View(scheduleSet);
        }

        //
        // POST: /Driver/Schedule
        [HttpPost]
        public ActionResult Schedule(ScheduleModelSet model)
        {
            var schedule = new Schedule();
            var newWeekSchedule = schedule.SaveDriverSchedule(model.ScheduleModels, model.IsDeviationSet);
            System.Web.HttpContext.Current.Session["Schedule"] = newWeekSchedule;

            return RedirectToAction("Schedule", "Driver", new { useLocalValues = true });
        }

        //
        // GET: /Driver/ScheduleDeviation
        [HttpGet]
        public ActionResult ScheduleDeviation(bool useLocalValues = false, int weeksFromNow = 0)
        {
            var thisFirstDay = DateTime.Now.StartOfWeek(DaysInfo.SwedishCultureInfo.DateTimeFormat.FirstDayOfWeek);
            var scheduleSet = new ScheduleModelSet
            {
                IsDeviationSet = true,
                CurrentDate = thisFirstDay.Date.AddDays(7 * weeksFromNow),
                WeeksFromNow = weeksFromNow
            };

            var weeks = new List<List<ScheduleModel>>();
            if (!useLocalValues)
            {
                var schedule = new Schedule();
                for (var i = 0; i < scheduleSet.NumberOfWeeks; i++)
                {
                    var futureFirstDay = thisFirstDay.AddDays(i * 7);
                    var futureLastDay = futureFirstDay.AddDays(6);
                    var driverSchedule = schedule.GetDriverSchedule(futureFirstDay, futureLastDay);
                    weeks.Add(driverSchedule);
                }
                System.Web.HttpContext.Current.Session["ScheduleWeeks"] = weeks;
            }
            else
            {
                weeks = System.Web.HttpContext.Current.Session["ScheduleWeeks"] as List<List<ScheduleModel>>;
            }
            scheduleSet.ScheduleModels = weeks[weeksFromNow];
            return View(scheduleSet);
        }

        //
        // POST: /Driver/ScheduleDeviation
        [HttpPost]
        public ActionResult ScheduleDeviation(ScheduleModelSet model)
        {
            var schedule = new Schedule();
            var newWeekSchedule = schedule.SaveDriverSchedule(model.ScheduleModels, model.IsDeviationSet);
            //Update sessions
            var weeks = System.Web.HttpContext.Current.Session["ScheduleWeeks"] as List<List<ScheduleModel>>;
            weeks[model.WeeksFromNow] = newWeekSchedule;
            System.Web.HttpContext.Current.Session["ScheduleWeeks"] = weeks;

            return RedirectToAction("ScheduleDeviation", "Driver", new { useLocalValues = true, weeksFromNow = model.WeeksFromNow });
        }
        /// <summary>
        /// Loads the next/previous week for the schedule deviation.
        /// </summary>
        /// <param name="weeksFromNow">current weeks from now</param>
        /// <param name="gotoNextWeek">true if load next week, false if previous week</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ScheduleLoadWeek(int weeksFromNow, bool gotoNextWeek)
        {
            //increase or decrease  week from now
            weeksFromNow = gotoNextWeek ? weeksFromNow + 1 : weeksFromNow - 1;
            return RedirectToAction("ScheduleDeviation", "Driver", new { useLocalValues = true, weeksFromNow = weeksFromNow });
        }

        public ActionResult PersonalInfo()
        {
            return View();
        }
    }
}
