using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;
using Moridge.BusinessLogic;
using Moridge.Extensions;
using Moridge.Models;
using Booking = Moridge.BusinessLogic.Booking;

namespace Moridge.Controllers
{
    [Authorize(Roles = RolesHelper.DriverRole + "," + RolesHelper.AdminRole)]
    public class DriverController : Controller
    {
        #region Booking Controllers

        public ActionResult BookingDay(string date = null, string message = null)
        {
            var events = new Booking().GetBookingsFromCalendar();
            System.Web.HttpContext.Current.Session["AllEvents"] = events;

            //setup date
            var today = Day.GetSwedishTime(DateTime.Now).ToString("yyyy-M-d");
            date = date ?? today;
            var isToday = date.Equals(today);
            var dateTime = Day.ConvertStringToDateTime(date);

            //setup bookings
            var bookingModel = new BookingDayModel(events, date) { DateTime = dateTime, IsToday = isToday };
            bookingModel.Booking.GetBookingsForOccasion(date, "Förmiddag");
            bookingModel.Booking.GetBookingsForOccasion(date, "Eftermiddag");

            //save any message
            bookingModel.Message = message;

            return View(bookingModel);
        }

        //
        // GET: /Driver/BookingCreate
        [HttpGet]
        public ActionResult BookingCreate(string parentPage, string parentDate = null)
        {
            return View(new BookingCreateModel { ParentPage = parentPage, ParentDate = parentDate });
        }

        //
        // POST: /Driver/BookingCreate
        [HttpPost]
        public ActionResult BookingCreate(BookingCreateModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var booking = new Booking();
            string companyName;
            booking.BookEvent(model, out companyName);
            var message = $"Skapade en ny körning {model.Date.ToShortDateString()} för {companyName}";
            return RedirectToAction(model.ParentPage, "Driver", new { date = model.ParentDate, message = message });
        }

        public ActionResult BookingWeek(string message = null)
        {
            var events = new Booking().GetBookingsFromCalendar();
            System.Web.HttpContext.Current.Session["AllEvents"] = events;

            return View(new BookingModel(events) { Date = Day.GetSwedishTime(DateTime.Now), Message = message });
        }

        public ActionResult BookingEvent(string eventId, string eventStatus, string parentDate = null)
        {
            var bookingEvent = new Booking().GetEvent(eventId);
            return View(new BookingEventModel
            {
                Event = bookingEvent,
                CurrentStatus = EventStatus.StringToStatus(eventStatus),
                ParentDate = parentDate
            });
        }

        public ActionResult BookingEventUpdate(BookingEventModel model)
        {
            //update the event
            new Booking().UpdateEvent(model.Event.Id, model.CurrentStatus.ToString());

            return RedirectToAction("BookingEvent", "Driver",
                new { eventId = model.Event.Id, eventStatus = model.CurrentStatus.ToString() });
        }

        #endregion

        #region Schedule Controllers

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
        public ActionResult ScheduleDeviation(bool useLocalValues = false, int weeksFromNow = 0, string date = null)
        {
            var today = Day.GetSwedishTime(DateTime.Now);
            var scheduleSet = new ScheduleModelSet
            {
                IsDeviationSet = true,
                CurrentDate = today.Date.AddDays(7 * weeksFromNow),
                WeeksFromNow = weeksFromNow
            };
            if (!string.IsNullOrEmpty(date))
            {
                scheduleSet.CurrentDate = Day.ConvertStringToDateTime(date);
            }
            var weeks = new List<List<ScheduleModel>>();
            if (!useLocalValues)
            {
                var schedule = new Schedule();
                for (var i = 0; i < scheduleSet.NumberOfWeeks; i++)
                {
                    var futureFirstDay = scheduleSet.CurrentDate.StartOfWeek(DaysInfo.SwedishCultureInfo.DateTimeFormat.FirstDayOfWeek).AddDays(i * 7);
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
            scheduleSet.ScheduleModels = weeks[weeksFromNow >= 4 ? 0 : weeksFromNow];
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

            return RedirectToAction("ScheduleDeviation", "Driver",
                new { useLocalValues = true, weeksFromNow = model.WeeksFromNow, date = model.CurrentDate.ToString("yyyy-M-d") });
        }

        /// <summary>
        /// Loads the next/previous week for the schedule deviation.
        /// </summary>
        /// <param name="weeksFromNow">current weeks from now</param>
        /// <param name="gotoNextWeek">true if load next week, false if previous week</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ScheduleLoadWeek(int weeksFromNow, bool gotoNextWeek, string date)
        {
            //increase or decrease  week from now
            weeksFromNow = gotoNextWeek ? weeksFromNow + 1 : weeksFromNow - 1;
            return RedirectToAction("ScheduleDeviation", "Driver",
                new { useLocalValues = true, weeksFromNow = weeksFromNow, date = date });
        }

        /// <summary>
        /// Loads the next/previous week for the schedule deviation.
        /// </summary>
        /// <param name="model">schedule model containing new date</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ScheduleLoadDate(ScheduleModelSet model)
        {
            return RedirectToAction("ScheduleDeviation", "Driver", new { useLocalValues = false, date = model.NewDate.ToString("yyyy-M-d") });
        }

        #endregion

        #region PersonalInfo Controllers

        [HttpGet]
        public ActionResult PersonalInfo()
        {
            var user = new DatabaseHelper().FindUser();
            return View(new DriverDetailsModel { Driver = user });
        }

        [HttpPost]
        public ActionResult PersonalInfo(DriverDetailsModel model)
        {
            //update user
            var updateResult = new DatabaseHelper().UpdateUser(model.Driver);
            return View(model);
        }

        #endregion

    }
}
