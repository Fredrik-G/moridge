using System;
using System.Collections.Generic;
using System.Globalization;
using Moridge.Extensions;

namespace Moridge.BusinessLogic
{
    /// <summary>
    /// Contains information and methods regarding days.
    /// </summary>
    public class DaysInfo
    { 
        public readonly CultureInfo SwedishCultureInfo = CultureInfo.GetCultureInfo(1053);

        public List<Day> Days { get; set; }

        public List<Day> AllDays(bool startFromToday = false)
        {
            if (Days == null)
            {
                Days = new List<Day>();
                var daysInWeek = Enum.GetNames(typeof (DayOfWeek)).Length;
                for (var i = 0; i < daysInWeek; i++)
                {
                    //TODO skippa helg?
                    if(startFromToday)
                    {
                        Days.Add(new Day {Date = DateTime.Now.AddDays(i)});
                    }
                    else
                    {
                       var monday = DateTime.Now.StartOfWeek(SwedishCultureInfo.DateTimeFormat.FirstDayOfWeek);
                       Days.Add(new Day {Date = monday.AddDays(i)});
                    }
                }
            }
            return Days;
        }

        public string GetDayString(DateTime day, bool isShort = false) 
            => SwedishCultureInfo.TextInfo.ToTitleCase(
                day.ToString(isShort ? "dddd" : "dddd, MMMM d, yyyy", SwedishCultureInfo));

        public string GetDayString(string day) => SwedishCultureInfo.TextInfo.ToTitleCase(day);
    }
}