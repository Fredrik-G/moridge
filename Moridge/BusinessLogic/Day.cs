using System;
using System.Collections.Generic;
using Google.Apis.Calendar.v3.Data;

namespace Moridge.BusinessLogic
{
    /// <summary>
    /// Contains information about a day.
    /// </summary>
    public class Day
    {
        public DateTime Date { get; set; }
        public string DayOfWeek { get; set; }
        public Dictionary<string, Occassion> Occassions { get; set; } 
        public string CurrentOccassion { get; set; }
        public Events EventsThisDay { get; set; }

        public Day()
        {
            var morning = new Occassion { Name = "Förmiddag" };
            var afternoon = new Occassion { Name = "Eftermiddag" };
            Occassions = new Dictionary<string, Occassion>
            {
                [morning.Name] = morning,
                [afternoon.Name] = afternoon
            };
        }

        /// <summary>
        /// Converts a date string to <see cref="DateTime"/>.
        /// date should be on format "yyyy-M-d".
        /// </summary>
        /// <param name="date">date to convert</param>
        /// <returns>the converted datetime object</returns>
        public static DateTime ConvertStringToDateTime(string date)
        {
            var splittedDate = date.Split('-');
            return new DateTime(Convert.ToInt16(splittedDate[0]), Convert.ToInt16(splittedDate[1]), Convert.ToInt16(splittedDate[2]));
        }

        /// <summary>
        /// Converts the given time to swedish time zone.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime GetSwedishTime(DateTime time)
        {
            var swedishTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(time.ToUniversalTime(), swedishTimeZone);
        }
    }
}