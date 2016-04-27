﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Google.Apis.Calendar.v3.Data;
using Moridge.BusinessLogic;

namespace Moridge.Models
{
    public class BookingModel
    {
        #region Data

        public Booking Booking { get; } = new Booking();

        #endregion

        public BookingModel(IList<Event> events)
        {
            Booking.Events = events;
        }

        public string GetDayString(DateTime day) => Booking.DaysInfo.GetDayString(day);
        public string GetMissingBookings(string date) => Booking.GetMissingBookings(date);
        public string GetTitle(bool isDetails) => isDetails ? Booking.Day.CurrentOccassion : "Boka";
        public List<Day> GetDays() => Booking.DaysInfo.AllDays(startFromToday: true);
        public Dictionary<string, Occassion> GetOccassions() => Booking.Day.Occassions;
        public IList<Event> GetEventsThisOccassion => Booking.Day.EventsThisDay.Items;     

        /// <summary>
        /// Gets the text for the header.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="occassion"></param>
        /// <returns></returns>
        public string GetHeaderText(string date, string occassion)
        {
            var upcomingOccassions = Booking.Day.Occassions[occassion].NumberOfBookings;
            Booking.Day.Occassions[occassion].NumberOfBookings = 0;//reset it after done.
            var numberOfBookingsForThisDriver = 4; //TODO
            return $"{upcomingOccassions} av {numberOfBookingsForThisDriver} bokningar";
        }
    }

    public class ScheduleModel
    {
        public Day CurrentDay {get;set;}
        public string CurrentOccassion {get;set;}
        public Schedule Schedule { get; } = new Schedule();

        public bool MorningActive { get; set; }
        public bool AfternoonActive{ get; set; }

        public string GetTitle() => "Arbetsschema";
        public List<Day> GetDays() => Schedule.GetDriverSchedule();

        public string GetDayString(DateTime day) => Schedule.DaysInfo.GetDayString(day, true);
        public string IsActive(Day day, string occassion) => day.Occassions[occassion].IsActive ? "selected" : string.Empty;

        public List<ScheduleDay> ScheduleDays { get; set; }
        public ScheduleModel(params ScheduleDay[] scheduleDays)
        {
            ScheduleDays = new List<ScheduleDay>();
            ScheduleDays.AddRange(scheduleDays);
        }
    }

    public class ScheduleDay
    {
        public string DayOfWeek { get; set; }
        public bool MorningActive { get; set; }
        public bool AfternoonActive { get; set; }
        public int MorningBookings { get; set; }
        public int AfternoonBookings { get; set; }
    }

    public class PersonalInfoModel { }
}
