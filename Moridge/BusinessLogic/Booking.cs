﻿using System;
using System.Collections.Generic;
using System.Linq;
using Google.Apis.Calendar.v3.Data;
using Moridge.Models;
using MyMoridgeServer.Models;

namespace Moridge.BusinessLogic
{
    /// <summary>
    /// Contains information and methods regarding booking.
    /// </summary>
    public class Booking
    {
        public DaysInfo DaysInfo { get; } = new DaysInfo();
        public Day Day { get; } = new Day();

        private Schedule _schedule;

        public IList<Event> Events { get; set; }

        /// <summary>
        /// Gets all bookings for the given occassion.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="occassion"></param>
        /// <returns></returns>
        public IList<Event> GetBookingsForOccasion(string date, string occassion)
        {
            Day.CurrentOccassion = occassion;
            var day = Day.ConvertStringToDateTime(date);
            Day.EventsThisDay = new Events { Items = new List<Event>() };
            Day.Occassions[occassion].EventsThisOccassion = new Events { Items = new List<Event>() };
            
            foreach (var bookingEvent in Events.Where(x => x.Start.DateTime.Value.Date.Equals(day.Date)))
            {
                if (IsTimeDuringOccassion(occassion, bookingEvent.Start.DateTime.Value))
                {
                    Day.EventsThisDay.Items.Add(bookingEvent);
                    Day.Occassions[occassion].EventsThisOccassion.Items.Add(bookingEvent);
                }
            }
            return Day.EventsThisDay.Items;
        }

        /// <summary>
        /// Checks if given time occurs during the given occassion.
        /// </summary>
        /// <param name="occassion"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        private bool IsTimeDuringOccassion(string occassion, DateTime time)
        {
            var swedishTime = Day.GetSwedishTime(time);
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
            return swedishTime.TimeOfDay >= start && swedishTime.TimeOfDay <= end;
        }

        /// <summary>
        /// Gets the number of missing bookings for this driver.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public string GetMissingBookings(string date)
        {
            int morning;
            int afternoon;

            //Get the number of scheduled bookings
            _schedule = _schedule ?? new Schedule();
            var driverScheduleBookings = _schedule.GetDriverScheduleBookings(date, out morning, out afternoon);

            //Get total bookings for this date
            Day.Occassions["Förmiddag"].NumberOfBookings += GetBookingsForOccasion(date, "Förmiddag").Count;
            Day.Occassions["Förmiddag"].BookingsForDriver += morning;
            Day.Occassions["Eftermiddag"].NumberOfBookings += GetBookingsForOccasion(date, "Eftermiddag").Count;
            Day.Occassions["Eftermiddag"].BookingsForDriver += afternoon;

            var totalNumberOfBookings = Day.Occassions["Förmiddag"].NumberOfBookings +
                                        Day.Occassions["Eftermiddag"].NumberOfBookings;

            //Subtract those already booked
            return (driverScheduleBookings - totalNumberOfBookings).ToString();
        }

        public void BookEvent(BookingCreateModel model)
        {
            BookingEvent bookingEvent = new BookingEvent();
            var date = Day.ConvertStringToDateTime(model.Date);
            bookingEvent.StartDateTime = date;
            bookingEvent.EndDateTime = date;

            bookingEvent.StartDateTime = bookingEvent.StartDateTime.AddHours(
                model.Occassion == BookingCreateModel.Occassions.Morning ? 0 : 4);
            bookingEvent.EndDateTime = bookingEvent.EndDateTime.AddHours(
                model.Occassion == BookingCreateModel.Occassions.Morning ? 4 : 8);

            bookingEvent.CustomerOrgNo = model.CustomerOrgNo;
            bookingEvent.CustomerEmail = "fredrikgummus@gmail.com";//model.CustomerEmail;
            bookingEvent.CustomerAddress = model.CustomerAddress;
            bookingEvent.VehicleRegNo = model.VehicleRegNo;
            bookingEvent.IsBooked = true;
            bookingEvent.CompanyName = "CompanyName";//model.CompanyName;
            bookingEvent.CustomerAddress = model.CustomerAddress;
            bookingEvent.BookingHeader = "Header";//bookingEventDTO.BookingHeader;
            bookingEvent.BookingMessage = model.BookingMessage;
            bookingEvent.ResourceId = 2;
            bookingEvent.SupplierEmailAddress = "fredrikgummus@gmail.com";//bookingEventDTO.SupplierEmailAddress;
            bookingEvent.Attendees = new List<string> { "fredrikgummus@gmail.com" }; //bookingEventDTO.Attendees;

            MyMoridgeServer.BusinessLogic.Booking booking = new MyMoridgeServer.BusinessLogic.Booking();
            booking.BookEvent(bookingEvent);
        }

        /// <summary>
        /// Gets the text for the header.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="occassion"></param>
        /// <returns></returns>
        public string GetHeaderText(string date, string occassion = "")
        {
            var upcomingOccassions = 0;
            var numberOfBookingsForThisDriver = 0;
            if (occassion.Equals(string.Empty))
            {
                foreach(var setOccassion in Day.Occassions)
                {
                    upcomingOccassions += setOccassion.Value.NumberOfBookings;
                    numberOfBookingsForThisDriver += setOccassion.Value.BookingsForDriver;

                    setOccassion.Value.NumberOfBookings = 0;//reset it after done.
                    setOccassion.Value.BookingsForDriver = 0;//reset it after done.
                }
            }
            else
            {
                upcomingOccassions = Day.Occassions[occassion].NumberOfBookings;
                numberOfBookingsForThisDriver = Day.Occassions[occassion].BookingsForDriver;
                Day.Occassions[occassion].NumberOfBookings = 0;//reset it after done.
                Day.Occassions[occassion].BookingsForDriver = 0;//reset it after done.
            }

            return $"{upcomingOccassions} av {numberOfBookingsForThisDriver} bokningar";
        }

    }
}