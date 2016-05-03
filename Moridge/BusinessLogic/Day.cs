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
        /// </summary>
        /// <param name="date">date to convert</param>
        /// <returns>the converted datetime object</returns>
        public static DateTime ConvertStringToDateTime(string date)
        {
            var splittedDate = date.Split('-');
            return new DateTime(Convert.ToInt16(splittedDate[0]), Convert.ToInt16(splittedDate[1]), Convert.ToInt16(splittedDate[2]));
        }
    }
}