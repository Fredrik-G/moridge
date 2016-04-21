using System;
using System.Collections.Generic;
using System.Linq;
using Google.Apis.Calendar.v3.Data;
using Moridge.Models;

namespace Moridge.BusinessLogic
{
    /// <summary>
    /// Contains information and methods regarding booking.
    /// </summary>
    public class Booking
    {
        public  Occassions Occassions {get;}= new Occassions();

        public IList<Event> Events { get; set; }
        public int TotalNumberOfBookings { get; set; }

        /// <summary>
        /// Gets all bookings for the given occassion.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="occassion"></param>
        /// <returns></returns>
        public IList<Event> GetBookingsForOccasion(string date, string occassion)
        {
            Occassions.CurrentOccassion = occassion;
            var splittedDate = date.Split('-');
            var day = new DateTime(Convert.ToInt16(splittedDate[0]), Convert.ToInt16(splittedDate[1]), Convert.ToInt16(splittedDate[2]));

            Occassions.EventsThisOccassion = new Events { Items = new List<Event>() };
            foreach (var bookingEvent in Events.Where(x => x.Start.DateTime.Value.Date.Equals(day.Date)))
            {
                if (IsTimeDuringOccassion(occassion, bookingEvent.Start.DateTime.Value))
                {
                    Occassions.EventsThisOccassion.Items.Add(bookingEvent);
                }
            }
            return Occassions.EventsThisOccassion.Items;
        }

        /// <summary>
        /// Checks if given time occurs during the given occassion.
        /// </summary>
        /// <param name="occassion"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        private bool IsTimeDuringOccassion(string occassion, DateTime time)
        {
            var swedishTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
            var swedishTime = TimeZoneInfo.ConvertTimeFromUtc(time.ToUniversalTime(), swedishTimeZone);
            var start = new TimeSpan();
            var end = new TimeSpan();
            if (occassion.Equals("Förmiddag"))
            {
                start = new TimeSpan(8, 0, 0);
                end = new TimeSpan(12, 0, 0);
            }
            else if (occassion.Equals("Eftermiddag"))
            {
                start = new TimeSpan(13, 0, 0);
                end = new TimeSpan(17, 0, 0);
            }
            else
            {
                //both false => is debug or some error
                //if(!debug) { database.logError(); }
            }
            return swedishTime.TimeOfDay >= start && swedishTime.TimeOfDay <= end;
        }

        /// <summary>
        /// Gets the number of missing bookings for this driver.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public string GetMissingBookings(string date)
        {
            TotalNumberOfBookings = 0;
            //Get total bookings for this date
            foreach (var occassion in Occassions.OccasionsPerDay)
            {
                TotalNumberOfBookings += GetBookingsForOccasion(date, occassion).Count;
            }
            //Subtract those already booked
            return (4 * 2 - TotalNumberOfBookings).ToString();
            //TODO
        }
    }
}