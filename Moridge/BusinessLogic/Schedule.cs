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

        public Schedule()
        {
            _dbContext = new ApplicationDbContext();
            _userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_dbContext));
            _user = _userManager.FindById(HttpContext.Current.User.Identity.Name);
        }

        /// <summary>
        /// Gets the driver's schedule.
        /// </summary>
        public List<Day> GetDriverSchedule()
        {
            ConvertDatabaseSchedule(_user.Schedule);
            return DaysInfo.Days;
        }

        /// <summary>
        /// Converts the database schedule to the local <see cref="Day"/> schedule.
        /// </summary>
        /// <param name="schedule">the database schedule</param>
        public void ConvertDatabaseSchedule(ICollection<DaySchedule> schedule)
        {
            DaysInfo.Days = new List<Day>();
            foreach(var daySchedule in schedule)
            {
                var day = new Day { DayOfWeek = daySchedule.DayOfWeek };
                day.Occassions["Förmiddag"].BookingsForDriver = daySchedule.Morning;
                day.Occassions["Förmiddag"].IsActive = daySchedule.MorningActive;
                day.Occassions["Eftermiddag"].BookingsForDriver = daySchedule.Afternoon;
                day.Occassions["Eftermiddag"].IsActive = daySchedule.AfternoonActive;
                DaysInfo.Days.Add(day);
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