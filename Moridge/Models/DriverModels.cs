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

        private List<DateTime> _days;

        public BookingModel(IList<Event> events)
        {
            Booking.Events = events;
        }

        public Booking Booking {get;} = new Booking();

        public List<DateTime> Days
        {
            get
            {
                if (_days == null)
                {
                    _days = new List<DateTime>();
                    var daysInWeek = Enum.GetNames(typeof (DayOfWeek)).Length;
                    for (var i = 0; i < daysInWeek; i++)
                    {
                        _days.Add(DateTime.Now.AddDays(i));
                        //TODO skippa helg?
                    }
                }
                return _days;
            }
        }

        #endregion

        public string GetDayString(DateTime day) => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(@day.ToString("dddd, MMMM d, yyyy"));
        public string GetMissingBookings(string date) => Booking.GetMissingBookings(date);
        public string GetTitle() => Booking.Occassions.CurrentOccassion;
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


    public class ScheduleModel { }
    public class PersonalInfoModel { }
}
