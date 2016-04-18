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

        public int GetBookingsForOccasion()
        {
            //TODO use real value
            //Events.Items.FirstOrDefault().Start.Date
            return 4; //chosen by a fair dice roll. guaranteed to be random.
        }

        public Events Events { get; set; }

        public string OccassionString(DateTime day, string occassion)
        {
            return day.DayOfWeek + (occassion.Equals("Förmiddag") ? "-AM" : "-PM");
        }
    }


    public class ScheduleModel { }
    public class PersonalInfoModel { }
}
