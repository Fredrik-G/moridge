using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
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
                day.Occassions["Eftermiddag"].BookingsForDriver = daySchedule.Afternoon;
                DaysInfo.Days.Add(day);
            }
        }

        /// <summary>
        /// Creates a default schedule for this driver.
        /// </summary>
        public void CreateDefaultSchedule()
        {
            var days = new List<DaySchedule>();
            var daysInWeek = Enum.GetNames(typeof (DayOfWeek)).Length;
            for (var i = 0; i < daysInWeek; i++)
            {
                var monday = DateTime.Now.StartOfWeek(DaysInfo.SwedishCultureInfo.DateTimeFormat.FirstDayOfWeek);
                //Use swedish names for days.
                var day = DaysInfo.SwedishCultureInfo.TextInfo.ToTitleCase(
                        monday.AddDays(i).ToString("dddd", DaysInfo.SwedishCultureInfo));
                days.Add(new DaySchedule{
                            DayOfWeek = day,
                            Morning = 4,
                            Afternoon = 4
                        });
            }
            _user.Schedule = days;
            _dbContext.SaveChanges();
        }
    }
}