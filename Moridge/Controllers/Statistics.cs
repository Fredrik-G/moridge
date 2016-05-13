using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Moridge.BusinessLogic;
using Moridge.Models;
using MyMoridgeServer.Models;

namespace Moridge.Controllers
{
    public class Statistics
    {
        /// <summary>
        /// Reads all booking events from the database and groups them by company.
        /// </summary>
        /// <returns>list of bookings</returns>
        public List<List<BookingEvent>> ReadBookingEvents()
        {
            var bookings = new MyMoridgeServer.BusinessLogic.Booking().GetAllBookings();
            var groupedBookings = bookings.GroupBy(x => x.CustomerOrgNo)
                .Select(group => group.ToList())
                .OrderByDescending(events => events.Count)
                .ToList();

            return groupedBookings;
        }
        
        /// <summary>
        /// Sets up the statistics models by calculating companies bookings.
        /// </summary>
        /// <returns>statistic model containing information about companies booking</returns>
        public StatisticsSetModel SetupModels()
        {
            var companiesBookings = ReadBookingEvents();
            var model = new StatisticsSetModel();
            foreach (var company in companiesBookings)
            {
                model.StatisticsModels.Add(new StatisticsModel(company));
            }
            return model;
        }

        /// <summary>
        /// Sets up the statistic chart.
        /// </summary>
        /// <param name="statisticsModel">statistic model containing data</param>
        /// <returns>chart model</returns>
        public StatisticsChart SetupChart(StatisticsModel statisticsModel)
        {
            //group up events on the same date
            var groupedEvents = statisticsModel.BookingEvents.GroupBy(e => e.StartDateTime)
                .Select(group => group.ToList())
                .ToList();

            //iterate grouped events and assign values
            var eventsCount = groupedEvents.Count;
            var xValue = new string[eventsCount];
            var yValues = new string[eventsCount];
            var firstDate = groupedEvents.First().First().StartDateTime;
            var lastDate = groupedEvents.Last().Last().EndDateTime;
            for (var i = 0; i < eventsCount; i++)
            {
                //assign x/y-values
                var isMorning = Booking.IsTimeDuringOccassion("Förmiddag", groupedEvents[i].First().StartDateTime, true);
                xValue[i] = groupedEvents[i].First().StartDateTime.ToString("yyyy-M-d") + $"{(isMorning ? "FM" : "EM")}";
                yValues[i] = groupedEvents[i].Count.ToString();

                //check dates
                if (groupedEvents[i].First().StartDateTime < firstDate)
                {
                    firstDate = groupedEvents[i].First().StartDateTime;
                    Debug.WriteLine("Ändrar firstdate");
                }
                if (groupedEvents[i].First().EndDateTime > lastDate)
                {
                    Debug.WriteLine("Ändrar lastdate");
                    lastDate = groupedEvents[i].First().StartDateTime;
                }
            }
            return new StatisticsChart
            {
                Dates = xValue,
                EventCount = yValues,
                CompanyName = statisticsModel.CompanyName,
                FirstDate = firstDate,
                LastDate = lastDate
            };
        }
    }
}