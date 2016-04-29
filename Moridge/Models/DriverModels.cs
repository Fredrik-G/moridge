using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Google.Apis.Calendar.v3.Data;
using Moridge.BusinessLogic;
using Moridge.Extensions;

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

    public class ScheduleModelSet
    {
        public List<ScheduleModel> ScheduleModels { get; set; }
        public DateTime CurrentDate { get; set; }
        public string CurrentWeek => GetCurrentWeek();

        private string GetCurrentWeek()
        {
            var swedishInfo = DaysInfo.SwedishCultureInfo;
            var weekNumber = swedishInfo.Calendar.GetWeekOfYear(
                CurrentDate,
                swedishInfo.DateTimeFormat.CalendarWeekRule,
                swedishInfo.DateTimeFormat.FirstDayOfWeek);
            var firstDayOfWeek = DateTime.Now.StartOfWeek(swedishInfo.DateTimeFormat.FirstDayOfWeek);
            var lastDayOfWeek = firstDayOfWeek.AddDays(6);

            return $"Vecka {weekNumber} - {firstDayOfWeek.Day}-{lastDayOfWeek.ToString("dd MMMM", swedishInfo.DateTimeFormat)}";
        }

        public string GetTitle() => "Arbetsschema";

        /// <summary>
        /// Sets the date to current week for the schedule models.
        /// </summary>
        public void SetCurrentWeek()
        {
            var monday = DateTime.Now.StartOfWeek(DaysInfo.SwedishCultureInfo.DateTimeFormat.FirstDayOfWeek);
            CurrentDate = monday;
            for(var i = 0; i < ScheduleModels.Count; i++)
            {
                ScheduleModels[i].Date = monday.AddDays(i);
            }
        }
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
