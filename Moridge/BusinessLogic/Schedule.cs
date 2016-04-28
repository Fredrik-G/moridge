using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Moridge.Extensions;
using Moridge.Models;

namespace Moridge.BusinessLogic
{
    /// <summary>
    /// Contains information and methods regarding scheduling.
    /// </summary>
    public class Schedule
    {
        public DaysInfo DaysInfo { get; } = new DaysInfo();

        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationUser _user;

        public Schedule(string userId = null)
        {
            _dbContext = new ApplicationDbContext();
            _userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_dbContext));
            _user = _userManager.FindById(userId ?? HttpContext.Current.User.Identity.Name);
        }

        /// <summary>
        /// Gets the driver's schedule.
        /// </summary>
        public List<ScheduleModel> GetDriverSchedule()
        {
            ConvertDatabaseSchedule(_user.Schedule);
            return DaysInfo.ScheduleDays;
        }

        /// <summary>
        /// Saves the given schedule back to the database.
        /// </summary>
        /// <param name="schedule">the schedule to save</param>
        /// <returns>gets the schedule as list</returns>
        public List<ScheduleModel> SaveDriverSchedule(IEnumerable<ScheduleModel> schedule)
        {
            for(var i = 0; i < _user.Schedule.Count; i++)
            {
                var dbDay = _user.Schedule.ElementAt(i);
                var newDay = schedule.ElementAt(i);

                dbDay.MorningActive = newDay.MorningActive;
                dbDay.AfternoonActive = newDay.AfternoonActive;
                dbDay.Morning = newDay.MorningBookings;
                dbDay.Afternoon = newDay.AfternoonBookings;

                newDay.DayOfWeek = dbDay.DayOfWeek;
            }
            _dbContext.SaveChanges();
            return schedule.ToList();
        }

        /// <summary>
        /// Converts the database schedule to the local <see cref="Day"/> schedule.
        /// </summary>
        /// <param name="schedule">the database schedule</param>
        public void ConvertDatabaseSchedule(ICollection<DaySchedule> schedule)
        {
            DaysInfo.ScheduleDays = new List<ScheduleModel>();
            foreach(var daySchedule in schedule)
            {
                var day = new ScheduleModel
                {
                    DayOfWeek = daySchedule.DayOfWeek, 
                    MorningActive = daySchedule.MorningActive,
                    AfternoonActive = daySchedule.AfternoonActive,
                    MorningBookings = daySchedule.Morning,
                    AfternoonBookings = daySchedule.Afternoon

                };
                DaysInfo.ScheduleDays.Add(day);
            }
        }

        /// <summary>
        /// Creates a default schedule for this driver.
        /// </summary>
        public void CreateDefaultSchedule()
        {
            var dayNames = DaysInfo.SwedishCultureInfo.DateTimeFormat.DayNames;
            dayNames = dayNames.Shift((int) DayOfWeek.Monday);
            const int weekdays = 5;
            const int weekendDays = 2;
            var days = new List<DaySchedule>();
            for (var i = 0; i < weekdays; i++)
            {
                days.Add(new DaySchedule{
                            DayOfWeek = DaysInfo.GetDayString(dayNames[i]),
                            Morning = 4,
                            Afternoon = 4,
                            MorningActive = true,
                            AfternoonActive = true
                        });
            }
            //handle weekends diffently
            for(var i = 0; i < weekendDays; i++)
            {
                days.Add(new DaySchedule{
                            DayOfWeek = DaysInfo.GetDayString(dayNames[weekdays + i]),
                            Morning = 0,
                            Afternoon = 0,
                            MorningActive = false,
                            AfternoonActive = false
                        });
            }
            _user.Schedule = days;
            _dbContext.SaveChanges();
        }
    }
}