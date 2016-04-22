using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Google.Apis.Calendar.v3.Data;
using Moridge.Models;

namespace Moridge.BusinessLogic
{
    /// <summary>
    /// Contains information and methods regarding days.
    /// </summary>
    public class DaysInfo
    { 
        private readonly CultureInfo _swedishCultureInfo = CultureInfo.GetCultureInfo(1053);

        private List<DateTime> _days;

        public List<DateTime> AllDays(bool startFromToday)
        {
            if (_days == null)
            {
                _days = new List<DateTime>();
                var daysInWeek = Enum.GetNames(typeof (DayOfWeek)).Length;
                for (var i = 0; i < daysInWeek; i++)
                {
                    //TODO skippa helg?
                    if(startFromToday)
                    {
                        _days.Add(DateTime.Now.AddDays(i));
                    }
                    else
                    {
                        var monday = DateTime.Now.StartOfWeek(_swedishCultureInfo.DateTimeFormat.FirstDayOfWeek);
                        _days.Add(monday.AddDays(i));
                    }
                }
            }
            return _days;
        }

        public string GetDayString(DateTime day, bool isShort = false) 
            => _swedishCultureInfo.TextInfo.ToTitleCase(
                @day.ToString(isShort ? "dddd" : "dddd, MMMM d, yyyy", _swedishCultureInfo));
    }
}