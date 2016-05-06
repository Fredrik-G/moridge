﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Google.Apis.Calendar.v3.Data;
using Moridge.BusinessLogic;
using Moridge.Extensions;

namespace Moridge.Models
{
    public class BookingModel
    {
        #region Data

        public Booking Booking { get; } = new Booking();
        public int BookingsForDriver { get; set; }
        public DateTime Date { get; set; }

        #endregion

        public BookingModel(IList<Event> events)
        {
            Booking.Events = events;
        }

        public string GetDayString(DateTime day) => Booking.DaysInfo.GetDayString(day);
        public string GetMissingBookings(string date) => Booking.GetMissingBookings(date);
        public string GetTitle(bool isDetails) => isDetails ? Booking.Day.CurrentOccassion : "Boka";
        public string GetHeaderText(string date, string occassion = "") => Booking.GetHeaderText(date, occassion);
        public List<Day> GetDays() => Booking.DaysInfo.AllDays(startFromToday: true);
        public Dictionary<string, Occassion> GetOccassions() => Booking.Day.Occassions;
        public IList<Event> GetEventsThisOccassion => Booking.Day.EventsThisDay.Items;
        public string CurrentWeek => GetCurrentWeek();

        private string GetCurrentWeek()
        {
            var swedishInfo = DaysInfo.SwedishCultureInfo;
            var weekNumber = swedishInfo.Calendar.GetWeekOfYear(
                            Date,
                            swedishInfo.DateTimeFormat.CalendarWeekRule,
                            swedishInfo.DateTimeFormat.FirstDayOfWeek);
            var firstDayOfWeek = Date.StartOfWeek(swedishInfo.DateTimeFormat.FirstDayOfWeek);
            var lastDayOfWeek = firstDayOfWeek.AddDays(6);

            return $"Vecka {weekNumber} - {firstDayOfWeek.Day}-{lastDayOfWeek.ToString("dd MMMM", swedishInfo.DateTimeFormat)}";
        }
    }

    public class BookingDayModel
    {        
        #region Data

        public Booking Booking { get; } = new Booking();
        public string Date { get; set; }
        public DateTime DateTime { get; set; }

        #endregion

        public BookingDayModel(IList<Event> events, string date)
        {
            Booking.Events = events;
            Date = date;
            Booking.GetMissingBookings(Date);
        }

        public string GetDayString() => Booking.DaysInfo.GetDayString(DateTime);
        public string GetTitle(bool isDetails) => isDetails ? Booking.Day.CurrentOccassion : "Boka";
        public IList<Event> GetEvents(string occassion) => Booking.Day.Occassions[occassion].EventsThisOccassion.Items;
        public Dictionary<string, Occassion> GetOccassions() => Booking.Day.Occassions;
        public int BookingsForDriver(string occassion) => Booking.Day.Occassions[occassion].BookingsForDriver;
        public int MissingBookings(string occassion) => Booking.Day.Occassions[occassion].BookingsForDriver -
                                                        Booking.Day.Occassions[occassion].NumberOfBookings;
    }

    public class BookingEventModel
    {
        public Event Event { get; set; }

        public string GetTitle() => "Bokningsdetaljer";
    }

    public class ScheduleModelSet
    {
        public bool IsDeviationSet { get; set; }

        /// <summary>
        /// List of all schedule models.
        /// </summary>
        public List<ScheduleModel> ScheduleModels { get; set; }

        /// <summary>
        /// The number of weeks from today.
        /// </summary>
        public int WeeksFromNow { get; set; }

        /// <summary>
        /// The number of future weeks to display and let the user change.
        /// </summary>
        public int NumberOfWeeks { get; set; } = 4;

        public DateTime CurrentDate { get; set; }
        public string CurrentWeek => GetCurrentWeek();
        public List<ScheduleModel> GetScheduleModels() => ScheduleModels;
        public string GetTitle() => "Arbetsschema";

        private string GetCurrentWeek()
        {
            var swedishInfo = DaysInfo.SwedishCultureInfo;
            var weekNumber = swedishInfo.Calendar.GetWeekOfYear(
                            CurrentDate,
                            swedishInfo.DateTimeFormat.CalendarWeekRule,
                            swedishInfo.DateTimeFormat.FirstDayOfWeek);
            var firstDayOfWeek = CurrentDate.StartOfWeek(swedishInfo.DateTimeFormat.FirstDayOfWeek);
            var lastDayOfWeek = firstDayOfWeek.AddDays(6);

            return $"Vecka {weekNumber} - {firstDayOfWeek.Day}-{lastDayOfWeek.ToString("dd MMMM", swedishInfo.DateTimeFormat)}";
        }

        public bool IsNextWeekAvailable() => WeeksFromNow + 1 < NumberOfWeeks;
        public bool IsPreviousWeekAvailable() => WeeksFromNow > 0;
    }

    public class ScheduleModel
    {
        public string DayOfWeek { get; set; }
        public bool MorningActive { get; set; }
        public bool AfternoonActive { get; set; }
        public int MorningBookings { get; set; }
        public int AfternoonBookings { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? Date { get; set; }
    }

    public class PersonalInfoModel { }
}
