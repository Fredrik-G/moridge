using System.Collections.Generic;
using System.Linq;
using Moridge.Models;
using MyMoridgeServer.Models;

namespace Moridge.BusinessLogic
{
    public class Statistics
    {
        /// <summary>
        /// Reads all booking events from the database and groups them by company.
        /// </summary>
        /// <param name="groupByDriver">true if group by driver, false if group by companies</param>
        /// <returns>list of bookings</returns>
        public List<List<BookingEvent>> ReadBookingEvents(bool groupByDriver = false)
        {
            var bookings = new MyMoridgeServer.BusinessLogic.Booking().GetAllBookings();
            var groupedBookings = bookings
                .GroupBy(x => groupByDriver ? x.SupplierEmailAddress : x.CustomerOrgNo)
                .Select(group => group.ToList())
                .OrderByDescending(events => events.Count)
                .ToList();

            return groupedBookings;
        }

        /// <summary>
        /// Sets up the statistics models by calculating companies bookings.
        /// </summary>
        /// <param name="bookingCount"></param>
        /// <param name="groupByDriver">true if group by driver, false if group by companies</param>
        /// <returns>statistic model containing information about companies booking</returns>
        public StatisticsSetModel SetupModels(out int bookingCount, bool groupByDriver = false)
        {
            var bookings = ReadBookingEvents(groupByDriver);
            var model = new StatisticsSetModel();
            bookingCount = 0;
            foreach (var booking in bookings)
            {
                var bookingModel = new StatisticsModel(booking);
                if (groupByDriver)
                {
                    bookingModel.User = UserHelper.GetUserByEmail(booking.First().SupplierEmailAddress) ??
                                        new User() { FirstName = "Förnamn", LastName = "Efternamn", Email = "notfound@removed"};
                }
                model.StatisticsModels.Add(bookingModel);
                bookingCount += booking.Count;
            }
            return model;
        }

        /// <summary>
        /// Sets up the statistic chart.
        /// </summary>
        /// <param name="model">statistic model containing data or null</param>
        /// <param name="models">list of statistic model containing data or null</param>
        /// <param name="useDates">true to use dates or false to use weekdays</param>
        /// <returns>chart model</returns>
        public StatisticsChart SetupChart(StatisticsModel model = null, List<StatisticsModel> models = null, bool useDates = true)
        {
            //group up events on the same date
            //model not null => chart is for one company
            if (model != null)
            {
                if (useDates)
                {
                    var groupedEvents = model.BookingEvents
                        .GroupBy(e => e.StartDateTime)
                        .Select(group => group.ToList())
                        .OrderByDescending(d => d.Count)
                        .ToList();
                    return GetChartValues(groupedEvents, "Datum för " + model.CompanyName, "Datum", true);
                }
                else
                {
                     var groupedEvents = model.BookingEvents
                        .GroupBy(e => e.StartDateTime.DayOfWeek)
                        .Select(group => group.ToList())
                        .ToList();
                    return GetChartValues(groupedEvents, "Dagar för " + model.CompanyName, "Dag", false);
                }
            }
            //chart is for total bookings
            else
            {
                if (useDates)
                {
                    var groupedEvents = models
                        .SelectMany(e => e.BookingEvents)
                        .GroupBy(x => x.StartDateTime)
                        .Select(group => group.ToList())
                        .ToList();

                    return GetChartValues(groupedEvents, "Datum för totalt", "Datum", true);
                }
                else
                {
                    var groupedEvents = models
                        .SelectMany(e => e.BookingEvents)
                        .GroupBy(x => x.StartDateTime.DayOfWeek)
                        .Select(group => group.ToList())
                        .OrderByDescending(d => d.Count)
                        .ToList();

                    return GetChartValues(groupedEvents, "Dagar för totalt", "Dag", false);
                }
            }
        }

        /// <summary>
        /// Gets values for the chart.
        /// </summary>
        /// <param name="bookingEvents">list of booking events</param>
        /// <param name="header">header text for the chart</param>
        /// <param name="xAxis">x-axis text for the c hart</param>
        /// <param name="useDates">true will show dates, false shows day of week</param>
        /// <returns>statistic chart</returns>
        private StatisticsChart GetChartValues(List<List<BookingEvent>> bookingEvents, string header, string xAxis, bool useDates = true)
        {
            var eventsCount = bookingEvents.Count;
            var xValue = new string[eventsCount];
            var yValues = new string[eventsCount];
            var firstDate = bookingEvents.First().First().StartDateTime;
            var lastDate = bookingEvents.Last().Last().EndDateTime;
            //iterate grouped events and assign values
            for (var i = 0; i < eventsCount; i++)
            {
                //assign x/y-values
                if (useDates)
                {
                    var isMorning = Booking.IsTimeDuringOccassion("Förmiddag", bookingEvents[i].First().StartDateTime, true);
                    xValue[i] = bookingEvents[i].First().StartDateTime.ToString("yyyy-M-d") +
                                $"{(isMorning ? "FM" : "EM")}";
                }
                else
                {
                    //Assign swedish day name in title case.
                    xValue[i] =
                        DaysInfo.SwedishCultureInfo.TextInfo.ToTitleCase(bookingEvents[i].First()
                            .StartDateTime.ToString("dddd", DaysInfo.SwedishCultureInfo));
                }
                yValues[i] = bookingEvents[i].Count.ToString();

                //check dates
                if (bookingEvents[i].First().StartDateTime < firstDate)
                {
                    firstDate = bookingEvents[i].First().StartDateTime;
                }
                if (bookingEvents[i].First().EndDateTime > lastDate)
                {
                    lastDate = bookingEvents[i].First().StartDateTime;
                }
            }
            return new StatisticsChart
            {
                Dates = xValue,
                EventCount = yValues,
                Header = header,
                XAxis = xAxis,
                FirstDate = firstDate,
                LastDate = lastDate
            };
        }
    }
}