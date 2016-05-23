using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using Google.Apis.Calendar.v3.Data;
using Moridge.BusinessLogic;
using Moridge.Extensions;

namespace Moridge.Models
{
    public class BookingModel
    {
        #region Data

        public Booking Booking { get; } = new Booking();
        public int BookingsForDriver { get; set; }
        public DateTime Date { get; set; }
        public string Message { get; set; }

        #endregion

        public BookingModel(IList<Event> events)
        {
            Booking.Events = events;
        }

        public string GetDayString(DateTime day) => Booking.DaysInfo.GetDayString(day);
        public string GetMissingBookings(string date) => Booking.GetMissingBookings(date);
        public string GetTitle(bool isDetails) => isDetails ? Booking.Day.CurrentOccassion : "Boka";
        public string GetHeaderText(string date, string occassion = "") => Booking.GetHeaderText(date, occassion);
        public List<Day> GetDays() => Booking.DaysInfo.AllDays(startFromToday: true);
        public Dictionary<string, Occassion> GetOccassions() => Booking.Day.Occassions;
        public IList<Event> GetEventsThisOccassion => Booking.Day.EventsThisDay.Items;
        public string CurrentWeek => GetCurrentWeek();

        private string GetCurrentWeek()
        {
            var swedishInfo = DaysInfo.SwedishCultureInfo;
            var weekNumber = swedishInfo.Calendar.GetWeekOfYear(
                            Date,
                            swedishInfo.DateTimeFormat.CalendarWeekRule,
                            swedishInfo.DateTimeFormat.FirstDayOfWeek);
            var firstDayOfWeek = Date.StartOfWeek(swedishInfo.DateTimeFormat.FirstDayOfWeek);
            var lastDayOfWeek = firstDayOfWeek.AddDays(6);

            return $"Vecka {weekNumber} - {firstDayOfWeek.Day}-{lastDayOfWeek.ToString("dd MMMM", swedishInfo.DateTimeFormat)}";
        }
    }

    public class BookingDayModel
    {        
        #region Data

        public Booking Booking { get; } = new Booking();
        public string Date { get; set; }
        public DateTime DateTime { get; set; }

        /// <summary>
        /// Determines if this is a model for this day.
        /// </summary>
        public bool IsToday { get; set; }

        public string Message { get; set; }

        #endregion

        public BookingDayModel(IList<Event> events, string date)
        {
            Booking.Events = events;
            Date = date;
            Booking.GetMissingBookings(Date);
        }

        public EventStatus.Status GetCurrentStatus(Event bookingEvent) => EventStatus.StringToStatus(bookingEvent.Summary);
        public string GetDayString() => Booking.DaysInfo.GetDayString(DateTime);
        public string GetTitle(bool isDetails) => isDetails ? Booking.Day.CurrentOccassion : "Boka";
        public IList<Event> GetEvents(string occassion) => Booking.Day.Occassions[occassion].EventsThisOccassion.Items;
        public Dictionary<string, Occassion> GetOccassions() => Booking.Day.Occassions;
        public int BookingsForDriver(string occassion) => Booking.Day.Occassions[occassion].BookingsForDriver;
        public int MissingBookings(string occassion) => Booking.Day.Occassions[occassion].BookingsForDriver -
                                                        Booking.Day.Occassions[occassion].NumberOfBookings;

        /// <summary>
        /// Checks if an event is done.
        /// </summary>
        /// <param name="occassion">the event's occassion</param>
        /// <param name="index">the event's index</param>
        /// <returns>true if done, otherwise false</returns>
        public bool EventIsDone(string occassion, int index)
        {
            var done = " " +EventStatus.Status.VehicleDelivered.ToString();
            var currentSummary = GetEvents(occassion)[index].Summary.Split('-');
            return currentSummary.Last().Equals(done);
        }
    }

    public class BookingCreateModel
    {
        public string ParentDate { get; set; }
        public string ParentPage { get; set; }
        public string DriverEmail { get; set; }
        public string ErrorMessage { get; set; }

        public string SelectedDriverEmail { get; set; }
        public IEnumerable<SelectListItem> Drivers { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime Date { get; set; }

        public const string _customerOrgNoDisplay = "Kund org. nummer (123456-7890)";
        public string CustomerOrgNoDisplay => _customerOrgNoDisplay;
        [Required]
        [Display(Name = _customerOrgNoDisplay)]
        public string CustomerOrgNo { get; set; }
        
        public const string _customerEmailDisplay = "Kunds email";
        public string CustomerEmailDisplay => _customerEmailDisplay;
        [Display(Name = _customerEmailDisplay)]
        public string CustomerEmail { get; set; }

        public const string _companyNameDisplay = "Företagsnamn";
        public string CompanyNameDisplay => _companyNameDisplay;
        [Display(Name = _companyNameDisplay)]
        public string CompanyName { get; set; }

        public const string _bookingHeaderDisplay = "Bokningsrubrik";
        public string BookingHeaderDisplay => _bookingHeaderDisplay;
        [Display(Name = _bookingHeaderDisplay)]
        public string BookingHeader { get; set; }

        private const string _vehicleRegNoDisplay = "Reg. nummer på fordon";
        public string VehicleRegNoDisplay => _vehicleRegNoDisplay;
        [Required]
        [Display(Name = _vehicleRegNoDisplay)]
        public string VehicleRegNo { get; set; }

        private const string _customerAddressDisplay = "Adress att hämta fordon";
        public string CustomerAddressDisplay => _customerAddressDisplay;
        [Required]
        [Display(Name = _customerAddressDisplay)]
        public string CustomerAddress { get; set; }

        private const string _messageDisplay = "Bokningsmeddelande";
        public string MessageDisplay => _messageDisplay;
        [Display(Name = _messageDisplay)]
        public string BookingMessage { get; set; }

        public Occasions.Occassions Occassion { get; set; }

        public string GetTitle() => "Boka";
    }

    public class BookingEventModel
    {
        public Event Event { get; set; }
        public EventStatus.Status CurrentStatus { get; set; } = EventStatus.Status.NotSet;
        public string ParentDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime NewDate { get; set; }

        public Occasions.Occassions Occassion { get; set; }

        public bool IsStatusAvailable(bool isNextStatus) => EventStatus.IsStatusValid(CurrentStatus, isNextStatus);

        public string GetTitle() => "Bokningsdetaljer";
    }

    public class ScheduleModelSet
    {
        public bool IsDeviationSet { get; set; }

        /// <summary>
        /// List of all schedule models.
        /// </summary>
        public List<ScheduleModel> ScheduleModels { get; set; }

        /// <summary>
        /// The number of weeks from today.
        /// </summary>
        public int WeeksFromNow { get; set; }

        /// <summary>
        /// The number of future weeks to display and let the user change.
        /// </summary>
        public int NumberOfWeeks { get; set; } = 4;

        public DateTime CurrentDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime NewDate { get; set; } = Day.GetSwedishTime(DateTime.Now);

        public string CurrentWeek => GetCurrentWeek();

        public List<ScheduleModel> GetScheduleModels() => ScheduleModels;
        public string GetTitle() => IsDeviationSet ? "Schemaavikelse" : "Arbetsschema";

        private string GetCurrentWeek()
        {
            var swedishInfo = DaysInfo.SwedishCultureInfo;
            var weekNumber = swedishInfo.Calendar.GetWeekOfYear(
                            CurrentDate,
                            swedishInfo.DateTimeFormat.CalendarWeekRule,
                            swedishInfo.DateTimeFormat.FirstDayOfWeek);
            var firstDayOfWeek = CurrentDate.StartOfWeek(swedishInfo.DateTimeFormat.FirstDayOfWeek);
            var lastDayOfWeek = firstDayOfWeek.AddDays(6);

            return $"Vecka {weekNumber} - {firstDayOfWeek.Day}-{lastDayOfWeek.ToString("dd MMM", swedishInfo.DateTimeFormat)}";
        }

        public bool IsNextWeekAvailable() => WeeksFromNow + 1 < NumberOfWeeks;
        public bool IsPreviousWeekAvailable() => WeeksFromNow > 0;
    }

    public class ScheduleModel
    {
        public string DayOfWeek { get; set; }
        public bool MorningActive { get; set; }
        public bool AfternoonActive { get; set; }
        public int MorningBookings { get; set; }
        public int AfternoonBookings { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? Date { get; set; }
    }

    public class PersonalInfoModel { }
}
