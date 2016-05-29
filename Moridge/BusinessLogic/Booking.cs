using System;
using System.Collections.Generic;
using System.Linq;
using Google.Apis.Calendar.v3.Data;
using Moridge.Extensions;
using Moridge.Models;
using MyMoridgeServer.BusinessLogic;
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
        /// <param name="isFromDatabase">wheter to use database time or default (calendar)</param>
        /// <returns></returns>
        public static bool IsTimeDuringOccassion(string occassion, DateTime time, bool isFromDatabase = false)
        {
            var swedishTime = Day.GetSwedishTime(time);
            var start = new TimeSpan();
            var end = new TimeSpan();
            if (occassion.Equals("Förmiddag"))
            {
                start = new TimeSpan(isFromDatabase ? 0 : 8, 0, 0);
                end = new TimeSpan(isFromDatabase ? 4 : 12, 0, 0);
            }
            else if (occassion.Equals("Eftermiddag"))
            {
                start = new TimeSpan(isFromDatabase ? 4 : 13, 0, 0);
                end = new TimeSpan(isFromDatabase ? 8 : 17, 0, 0);
            }
            else
            {
                //both false => is debug or some error
                Logger.LogError($"IsTimeDuringOccassion: invalid parameter {occassion}.");
            }
            return swedishTime.TimeOfDay >= start && swedishTime.TimeOfDay < end;
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

        /// <summary>
        /// Books a new event with values from the model.
        /// </summary>
        /// <param name="model">booking model containing event details</param>
        /// <param name="successful">out parameter showing creation success</param>
        /// <returns>the company name for this booking</returns>
        public string BookEvent(BookingCreateModel model, out bool successful)
        {
            //check if the current driver has available bookings this day.
            if (!DriverHasAvailableBookings(model))
            {
                //driver has no available bookings this day => don't create new booking
                successful = false;
                return string.Empty;
            }

            var bookingEvent = new BookingEvent();
            bookingEvent.StartDateTime = model.Date;
            bookingEvent.EndDateTime = model.Date;

            bookingEvent.StartDateTime = bookingEvent.StartDateTime.AddHours(
                model.Occassion == Occasions.Occassions.Morning ? 0 : 4);
            bookingEvent.EndDateTime = bookingEvent.EndDateTime.AddHours(
                model.Occassion == Occasions.Occassions.Morning ? 4 : 8);

            //Set up company information based off the org number.
            string customerEmail, companyName;
            GetCustomerDetails(model.CustomerOrgNo, out companyName, out customerEmail);

            bookingEvent.CustomerOrgNo = model.CustomerOrgNo;
            bookingEvent.CompanyName = companyName;
            bookingEvent.CustomerEmail = customerEmail;
            bookingEvent.CustomerAddress = model.CustomerAddress;
            bookingEvent.VehicleRegNo = model.VehicleRegNo;

            bookingEvent.IsBooked = true;
            bookingEvent.BookingHeader = model.BookingHeader;
            bookingEvent.BookingMessage = model.BookingMessage;
            bookingEvent.ResourceId = 1; //TODO
            bookingEvent.SupplierEmailAddress = model.SelectedDriverEmail;
            bookingEvent.Attendees = new List<string> { Common.GetAppConfigValue("MoridgeOrganizerCalendarEmail") };

            var booking = new MyMoridgeServer.BusinessLogic.Booking();
            booking.BookEvent(bookingEvent);

            successful = true;
            return companyName;
        }

        /// <summary>
        /// Checks if the driver has available bookings this day.
        /// </summary>
        /// <param name="model">booking model containing information about the driver and the bookings</param>
        /// <returns>true if the driver has available bookings, otherwise false</returns>
        private bool DriverHasAvailableBookings(BookingCreateModel model)
        {
            Events = new Booking().GetBookingsFromCalendar(model.SelectedDriverEmail);
            int morningScheduleBookings, afternoonScheduleBookings;
            _schedule = _schedule ?? new Schedule();
            _schedule.GetDriverScheduleBookings(model.Date.ToString("yyyy-M-d"), out morningScheduleBookings, out afternoonScheduleBookings);
            var bookingsThisOccasion = GetBookingsForOccasion(model.Date.ToString("yyyy-M-d"), model.Occassion.GetDisplayName()).Count;
            if (model.Occassion == Occasions.Occassions.Morning && (morningScheduleBookings - bookingsThisOccasion) <= 0 ||
                model.Occassion == Occasions.Occassions.Afternoon && (afternoonScheduleBookings - bookingsThisOccasion) <= 0)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Gets customer details based of given company name. 
        /// </summary>
        /// <param name="orgNumber">customer's orginsation number</param>
        /// <param name="companyName">name of company</param>
        /// <param name="customerEmail">customer's email</param>
        private void GetCustomerDetails(string orgNumber, out string companyName, out string customerEmail)
        {
            //TODO
            companyName = "Företag AB";
            customerEmail = "företagsemail@todo.notdone";
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

        /// <summary>
        /// Read bookings for the current user from the calendar.
        /// </summary>
        /// <param name="driverEmail">driver's email to read or null for current driver</param>
        /// <returns>list of bookings</returns>
        public IList<Event> GetBookingsFromCalendar(string driverEmail = null)
        {
            var calendar = new GoogleCalendar(Common.GetAppConfigValue("MoridgeOrganizerCalendarEmail"), Common.GetAppConfigValue("MoridgeMainCalendarEmail"));

            driverEmail = driverEmail ?? UserHelper.GetCurrentUser().Email;
            var events = calendar.GetUpcomingEventsForDriver(driverEmail);
            return events;
        }

        /// <summary>
        /// Reads an event from the calendar based on the given eventId.
        /// </summary>
        /// <param name="eventId">id for the event</param>
        /// <returns>found event</returns>
        public Event GetEvent(string eventId)
        {
            var calendar = new GoogleCalendar(Common.GetAppConfigValue("MoridgeOrganizerCalendarEmail"), Common.GetAppConfigValue("MoridgeMainCalendarEmail"));
            return calendar.GetEvent(eventId);
        }

        /// <summary>
        /// Updates a given event with the given status.
        /// </summary>
        /// <param name="eventId">id for the event</param>
        /// <param name="status">status message for the event</param>
        public void UpdateEvent(string eventId, string status)
        {
            var calendar = new GoogleCalendar(Common.GetAppConfigValue("MoridgeOrganizerCalendarEmail"), Common.GetAppConfigValue("MoridgeMainCalendarEmail"));
            calendar.UpdateEvent(eventId, status);
        }

        /// <summary>
        /// Deletes a given event.
        /// </summary>
        /// <param name="id">event id</param>
        public void DeleteEvent(string id)
        {
            var calendar = new GoogleCalendar(Common.GetAppConfigValue("MoridgeOrganizerCalendarEmail"), Common.GetAppConfigValue("MoridgeMainCalendarEmail"));
            calendar.DeleteEvent(id);
        }

        /// <summary>
        /// Moves an event to a new time.
        /// </summary>
        /// <param name="eventId">id for the event</param>
        /// <param name="newDate">new date to move to</param>
        /// <param name="occassion">morning or afternoon</param>
        /// <returns>the updated event</returns>
        public Event MoveEvent(string eventId, DateTime newDate, string occassion)
        {
            var calendar = new GoogleCalendar(Common.GetAppConfigValue("MoridgeOrganizerCalendarEmail"), Common.GetAppConfigValue("MoridgeMainCalendarEmail"));
            return calendar.MoveEvent(eventId, newDate, occassion);
        }

        /// <summary>
        /// Sorts the bookings by their status.
        /// </summary>
        public void SortBookingsByStatus()
        {
            foreach (var occassion in Day.Occassions)
            {
                occassion.Value.EventsThisOccassion.Items =
                    occassion.Value.EventsThisOccassion.Items.OrderBy(x => EventStatus.StringToStatus(x.Summary)).ToList();
            }
        }
    }
}
