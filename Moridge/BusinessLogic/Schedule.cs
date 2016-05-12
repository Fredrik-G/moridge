using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly DatabaseHelper _dbHelper;
        private readonly ApplicationUser _user;

        public Schedule(string userId = null, DatabaseHelper dbHelper = null)
        {
            _dbHelper = dbHelper ?? new DatabaseHelper();
            _user = _dbHelper.FindUser(userId);
        }

        /// <summary>
        /// Gets the driver's schedule.
        /// </summary>
        public List<ScheduleModel> GetDriverSchedule(DateTime? firstDay = null, DateTime? lastDay = null)
        {
            ConvertDatabaseSchedule(_user.Schedule, _user.ScheduleDeviation, firstDay, lastDay);
            return DaysInfo.ScheduleDays;
        }

        /// <summary>
        /// Converts the database schedule to the local <see cref="Day"/> schedule.
        /// </summary>
        /// <param name="schedule">the database schedule</param>
        /// <param name="scheduleDeviations">schedule deviation or null if ignore deviations</param>
        public void ConvertDatabaseSchedule(ICollection<DaySchedule> schedule,
            ICollection<ScheduleDeviation> scheduleDeviations, DateTime? firstDay, DateTime? lastDay)
        {
            DaysInfo.ScheduleDays = new List<ScheduleModel>();
            for(var i = 0; i < schedule.Count; i++)
            {
                var daySchedule = schedule.ElementAt(i);
                var deviations = scheduleDeviations?.Where(x => x.DayOfWeek == daySchedule.DayOfWeek).ToList();
                var deviationToday = GetDeviationForWeek(deviations, firstDay, lastDay);
                
                var day = new ScheduleModel
                {
                    Date = firstDay?.Date.AddDays(i),
                    DayOfWeek = daySchedule.DayOfWeek, 
                    MorningActive =  deviationToday?.MorningActive ?? daySchedule.MorningActive,
                    AfternoonActive = deviationToday?.AfternoonActive ?? daySchedule.AfternoonActive,
                    MorningBookings = deviationToday?.Morning ?? daySchedule.Morning,
                    AfternoonBookings = deviationToday?.Afternoon ?? daySchedule.Afternoon
                };
                DaysInfo.ScheduleDays.Add(day);
            }
        }

        /// <summary>
        /// Gets the deviation for given week or null if no deviation.
        /// </summary>
        /// <param name="deviations">list of all deviations</param>
        /// <param name="firstDay">start of week</param>
        /// <param name="lastDay">end of week</param>
        /// <returns>deviation or null</returns>
        private ScheduleDeviation GetDeviationForWeek(List<ScheduleDeviation> deviations, DateTime? firstDay, DateTime? lastDay)
        {
            return deviations?.FirstOrDefault(x => x.Date.Value >= firstDay && x.Date.Value <= lastDay);
        }

        /// <summary>
        /// Saves the given schedule back to the database.
        /// </summary>
        /// <param name="schedule">the schedule to save</param>
        /// <param name="isDeviationSet">determines if this set is a devation schedule or a normal schedule</param>
        /// <returns>gets the schedule as list</returns>
        public List<List<ScheduleModel>> SaveDriverSchedule(List<List<ScheduleModel>> schedule, bool isDeviationSet)
        {
            //Iterate each week and every day.
            foreach (var scheduleWeek in schedule)
            {
                for (var i = 0; i < _user.Schedule.Count; i++)
                {
                    var newDay = scheduleWeek.ElementAt(i);
                    var normalSchedule = _user.Schedule.ElementAt(i);
                    newDay.DayOfWeek = normalSchedule.DayOfWeek;

                    if (isDeviationSet)
                    {
                        SaveDeviation(normalSchedule, newDay);
                    }
                    else
                    {
                        normalSchedule.MorningActive = newDay.MorningActive;
                        normalSchedule.AfternoonActive = newDay.AfternoonActive;
                        normalSchedule.Morning = newDay.MorningBookings;
                        normalSchedule.Afternoon = newDay.AfternoonBookings;
                    }
                }
            }
            _dbHelper.Save();
            return schedule;
        }
        
                /// <summary>
        /// Saves the given schedule back to the database.
        /// </summary>
        /// <param name="schedule">the schedule to save</param>
        /// <param name="isDeviationSet">determines if this set is a devation schedule or a normal schedule</param>
        /// <returns>gets the schedule as list</returns>
        public List<ScheduleModel> SaveDriverSchedule(List<ScheduleModel> schedule, bool isDeviationSet)
        {
            for (var i = 0; i < _user.Schedule.Count; i++)
            {
                var newDay = schedule.ElementAt(i);
                var normalSchedule = _user.Schedule.ElementAt(i);
                newDay.DayOfWeek = normalSchedule.DayOfWeek;

                if (isDeviationSet)
                {
                    SaveDeviation(normalSchedule, newDay);
                }
                else
                {
                    normalSchedule.MorningActive = newDay.MorningActive;
                    normalSchedule.AfternoonActive = newDay.AfternoonActive;
                    normalSchedule.Morning = newDay.MorningBookings;
                    normalSchedule.Afternoon = newDay.AfternoonBookings;
                }
            }
            _dbHelper.Save();
            return schedule;
        }
        
        /// <summary>
        /// Saves the given deviation to the database.
        /// Only adds a new deviation if its different from the normal schedule.
        /// Edits any current deviation on the given date.
        /// </summary>
        /// <param name="normalSchedule">the normal schedule for this day</param>
        /// <param name="newDay">the new schedule to save</param>
        private void SaveDeviation(DaySchedule normalSchedule, ScheduleModel newDay)
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

            //Edit deviation if it exists on this date
            var index = _user.ScheduleDeviation.ToList().FindIndex(x => x.Date == newDay.Date);
            if (index != -1)
            {
                var existingDeviation = _user.ScheduleDeviation.ElementAt(index);
                existingDeviation.MorningActive = newDay.MorningActive;
                existingDeviation.AfternoonActive = newDay.AfternoonActive;
                existingDeviation.Morning = newDay.MorningBookings;
                existingDeviation.Afternoon = newDay.AfternoonBookings;
            }
            //Only add it if its different from the normal schedule.
            else if (IsScheduleDifferent(normalSchedule, scheduleDeviation))
            {
                _user.ScheduleDeviation.Add(scheduleDeviation);
            }
        }

        /// <summary>
        /// Checks if the schedule is different from the normal day schedule.
        /// </summary>
        /// <param name="normalDaySchedule">the normal schedule for a day</param>
        /// <param name="scheduleDayDeviation">the different schedule</param>
        /// <returns>true if different schedule</returns>
        private bool IsScheduleDifferent(DaySchedule normalDaySchedule, ScheduleDeviation scheduleDayDeviation)
        {
            return !(normalDaySchedule.DayOfWeek == scheduleDayDeviation.DayOfWeek &&
                   normalDaySchedule.MorningActive == scheduleDayDeviation.MorningActive &&
                   normalDaySchedule.AfternoonActive == scheduleDayDeviation.AfternoonActive &&
                   normalDaySchedule.Morning == scheduleDayDeviation.Morning &&
                   normalDaySchedule.Afternoon == scheduleDayDeviation.Afternoon);
        }

        /// <summary>
        /// Gets the number of scheduled bookings for this driver on a given date.
        /// </summary>
        /// <param name="date">date to check bookings on</param>
        /// <param name="morning">out value for the number of morning bookings</param>
        /// <param name="afternoon">out value for the number of morning bookings</param>
        /// <returns>total number of bookings (morning+afternoon)</returns>
        public int GetDriverScheduleBookings(string date, out int morning, out int afternoon)
        {
            var day = Day.ConvertStringToDateTime(date);
            var scheduleDeviation = _user.ScheduleDeviation.FirstOrDefault(x => x.Date.Value.Date.Equals(day));

            var normalSchedule = _user.Schedule.FirstOrDefault(
                x => x.DayOfWeek.ToLower().Equals(
                    DaysInfo.SwedishCultureInfo.DateTimeFormat.GetDayName(day.DayOfWeek)));

            morning = afternoon = 0;
            var useDeviation = false;
            if (scheduleDeviation != null)
            {
                //only add if the occassion is active
                morning += scheduleDeviation.MorningActive.Value ? scheduleDeviation.Morning.Value : 0;
                afternoon += scheduleDeviation.AfternoonActive.Value ? scheduleDeviation.Afternoon.Value : 0;
                useDeviation = true;
            }
            //no deviations found => use normal schedule
            if (!useDeviation && normalSchedule != null)
            {
                if (normalSchedule.MorningActive)
                {
                    morning += normalSchedule.Morning;
                }
                if (normalSchedule.AfternoonActive)
                {
                    afternoon += normalSchedule.Afternoon;
                }
            }
            return morning + afternoon;
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
            _dbHelper.Save();
        }
    }
}