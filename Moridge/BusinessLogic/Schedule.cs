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
        /// <param name="isDeviationSet">determines if this set is a devation schedule or a normal schedule</param>
        /// <returns>gets the schedule as list</returns>
        public List<ScheduleModel> SaveDriverSchedule(IEnumerable<ScheduleModel> schedule, bool isDeviationSet)
        {
            for(var i = 0; i < _user.Schedule.Count; i++)
            {
                var newDay = schedule.ElementAt(i);
                var normalSchedule = _user.Schedule.ElementAt(i);
                newDay.DayOfWeek = normalSchedule.DayOfWeek;

                if (isDeviationSet)
                {
                    var scheduleDeviation = new ScheduleDeviation
                    {
                        DayOfWeek = normalSchedule.DayOfWeek,
                        MorningActive = newDay.MorningActive,
                        AfternoonActive = newDay.AfternoonActive,
                        Morning = newDay.MorningBookings,
                        Afternoon = newDay.AfternoonBookings,
                        Date = newDay.Date
                    };
                    //Only add it if its different from the normal schedule.
                    if (IsScheduleDifferent(normalSchedule, scheduleDeviation))
                    {
                        _user.ScheduleDeviation.Add(scheduleDeviation);
                    }
                }
                else
                {
                    normalSchedule.MorningActive = newDay.MorningActive;
                    normalSchedule.AfternoonActive = newDay.AfternoonActive;
                    normalSchedule.Morning = newDay.MorningBookings;
                    normalSchedule.Afternoon = newDay.AfternoonBookings;
                }
            }
            _dbContext.SaveChanges();
            return schedule.ToList();
        }

        /// <summary>
        /// Checks if the schedule is different from the normal day schedule.
        /// </summary>
        /// <param name="normalDaySchedule">the normal schedule for a day</param>
        /// <param name="scheduleDayDeviation">the different schedule</param>
        /// <returns>true if different schedule</returns>
        private bool IsScheduleDifferent(DaySchedule normalDaySchedule, ScheduleDeviation scheduleDayDeviation)
        {
            return !((normalDaySchedule.DayOfWeek == scheduleDayDeviation.DayOfWeek) &&
                   (normalDaySchedule.MorningActive == scheduleDayDeviation.MorningActive) &&
                   (normalDaySchedule.AfternoonActive == scheduleDayDeviation.AfternoonActive) &&
                   (normalDaySchedule.Morning == scheduleDayDeviation.Morning) &&
                   (normalDaySchedule.Afternoon == scheduleDayDeviation.Afternoon));
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