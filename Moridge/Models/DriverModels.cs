using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Google.Apis.Calendar.v3.Data;

namespace Moridge.Models
{
    public class BookingModel
    {
        private List<DateTime> _days;
        public List<DateTime> Days
        {
            get
            {
                if (_days == null)
                {
                    _days = new List<DateTime>();
                    var daysInWeek = Enum.GetNames(typeof(DayOfWeek)).Length;
                    for (var i = 0; i < daysInWeek; i++)
                    {
                        _days.Add(DateTime.Now.AddDays(i));
                        //TODO skippa helg?
                    }
                }
                return _days;
            }
        }

        public string DayString(DateTime day)
        {
            return day.DayOfWeek + ", " + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(@day.ToString("MMMM d, yyyy"));
        }

        public string[] OccasionsPerDay
        {
            get
            {
                return new[] { "Förmiddag", "Eftermiddag" };
            }
        }

        public int GetBookingsForOccasion(DateTime day, string occassion)
        {
            var eventsThisOccassion = new Events { Items = new List<Event>() };
            foreach (var bookingEvent in Events.Items.Where(x => x.Start.DateTime.Value.Date.Equals(day.Date)))
            {
                if (IsTimeDuringOccassion(occassion, bookingEvent.Start.DateTime.Value))
                {
                    eventsThisOccassion.Items.Add(bookingEvent);
                }
            }


            return eventsThisOccassion.Items.Count;
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

        public Events Events { get; set; }
    }


    public class ScheduleModel { }
    public class PersonalInfoModel { }
}
