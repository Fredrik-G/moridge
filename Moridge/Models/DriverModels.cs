using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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

        public string GetDayString(DateTime day) => Booking.Days.GetDayString(day);
        public string GetMissingBookings(string date) => Booking.GetMissingBookings(date);
        public string GetTitle(bool isDetails) => isDetails ? Booking.Occassions.CurrentOccassion : "Boka";
        public List<DateTime> GetDays() => Booking.Days.AllDays(startFromToday: true);
        public string[] GetOccasionsPerDay() => Booking.Occassions.OccasionsPerDay;
        public IList<Event> GetEventsThisOccassion => Booking.Occassions.EventsThisOccassion.Items;     

        /// <summary>
        /// Gets the text for the header.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="occassion"></param>
        /// <returns></returns>
        public string GetHeaderText(string date, string occassion)
        {
            var upcomingOccassions = Booking.GetBookingsForOccasion(date, occassion).Count;
            var numberOfBookingsForThisDriver = 4; //TODO
            return $"{upcomingOccassions} av {numberOfBookingsForThisDriver} bokningar";
        }
    }

    public class ScheduleModel
    {
        public Schedule Schedule { get; } = new Schedule();

        public string GetTitle() => "Arbetsschema";
        public List<DateTime> GetDays() => Schedule.Days.AllDays(startFromToday: false);
    }

    public class PersonalInfoModel { }
}
