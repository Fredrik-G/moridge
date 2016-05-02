using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Google.Apis.Calendar.v3.Data;
using Moridge.BusinessLogic;
using Moridge.Extensions;
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


        //
        // GET: /Driver/Schedule
        [HttpGet]
        public ActionResult Schedule(bool useLocalValues = false)
        {
            var schedule = new Schedule();
            var driverSchedule = useLocalValues ? System.Web.HttpContext.Current.Session["Schedule"] as List<ScheduleModel>
                                                : schedule.GetDriverSchedule();
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
            var schedule = new Schedule();
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
