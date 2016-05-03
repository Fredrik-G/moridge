using System;
using System.Collections.Generic;
using System.Linq;
using Google.Apis.Calendar.v3.Data;

namespace Moridge.BusinessLogic
{
    /// <summary>
    /// Contains information and methods regarding booking.
    /// </summary>
    public class Booking
    {
        public DaysInfo DaysInfo { get; } = new DaysInfo();
        public Day Day { get; } = new Day();

        private readonly Schedule _schedule = new Schedule();

        public IList<Event> Events { get; set; }

        /// <summary>
        /// Gets all bookings for the given occassion.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="occassion"></param>
        /// <returns></returns>
        public IList<Event> GetBookingsForOccasion(string date, string occassion)
        {
            Day.CurrentOccassion = occassion;
            var day = Day.ConvertStringToDateTime(date);
            Day.EventsThisDay = new Events { Items = new List<Event>() };
            foreach (var bookingEvent in Events.Where(x => x.Start.DateTime.Value.Date.Equals(day.Date)))
            {
                if (IsTimeDuringOccassion(occassion, bookingEvent.Start.DateTime.Value))
                {
                    Day.EventsThisDay.Items.Add(bookingEvent);
                }
            }
            return Day.EventsThisDay.Items;
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
            //Get total bookings for this date
            var totalNumberOfBookings = 0;
            foreach (var occassion in Day.Occassions)
            {
                occassion.Value.NumberOfBookings += GetBookingsForOccasion(date, occassion.Value.Name).Count;
                totalNumberOfBookings += occassion.Value.NumberOfBookings;
            }
            //Subtract those already booked
            var driverScheduleBookings = _schedule.GetDriverScheduleBookings(date);
            return (driverScheduleBookings - totalNumberOfBookings).ToString();
        }

    }
}