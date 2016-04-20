using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using Google.Apis.Calendar.v3.Data;

namespace Moridge.Models
{
    public class BookingModel
    {
        #region Data

        private List<DateTime> _days;

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

        public string CurrentOccassion { get; set; }

        public string[] OccasionsPerDay
        {
            get { return new[] {"Förmiddag", "Eftermiddag"}; }
        }

        public IList<Event> Events { get; set; }
        public Events EventsThisOccassion { get; set; }

        #endregion

        public string DayString(DateTime day)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(@day.ToString("dddd, MMMM d, yyyy"));
        }

        public IList<Event> GetBookingsForOccasion(string date, string occassion)
        {
            CurrentOccassion = occassion;
            var splittedDate = date.Split('-');
            var day = new DateTime(Convert.ToInt16(splittedDate[0]), Convert.ToInt16(splittedDate[1]), Convert.ToInt16(splittedDate[2]));

            EventsThisOccassion = new Events { Items = new List<Event>() };
            foreach (var bookingEvent in Events.Where(x => x.Start.DateTime.Value.Date.Equals(day.Date)))
            {
                if (IsTimeDuringOccassion(occassion, bookingEvent.Start.DateTime.Value))
                {
                    EventsThisOccassion.Items.Add(bookingEvent);
                }
            }
            return EventsThisOccassion.Items;
        }

        /// <summary>
        /// Checks if given time occurs during the given occassion.
        /// </summary>
        /// <param name="occassion"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        private bool IsTimeDuringOccassion(string occassion, DateTime time)
        {
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
            return time.TimeOfDay >= start && time.TimeOfDay <= end;
        }
    }


    public class ScheduleModel { }
    public class PersonalInfoModel { }
}
