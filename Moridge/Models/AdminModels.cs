using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Moridge.BusinessLogic;
using MyMoridgeServer.Models;

namespace Moridge.Models
{
    public class DriverRegistryModel
    {
        public List<ApplicationUser> Drivers { get; set; } 
    }

    public class DriverDetailsModel
    {
        public ApplicationUser Driver { get; set; } = new ApplicationUser();
        public int Index { get; set; }

        /// <summary>
        /// Determines if this model is for creating a new user.
        /// </summary>
        public bool IsCreatingNew { get; set; } = false;

        public RolesHelper.Roles Roles { get; set; }

        [Display(Name = "Förnamn")]
        public string FirstName
        {
            get { return Driver.FirstName; }
            set { Driver.FirstName = value; }
        }

        [Display(Name="Efternamn")]
        public string LastName
        {
            get { return Driver.LastName; }
            set { Driver.LastName = value; }
        }

        [Display(Name="E-post")]
        public string Email 
        {
            get { return Driver.Email; }
            set
            {
                Driver.Email = value;
                Driver.UserName = value;
            }
        }

        [Display(Name="Adress")]
        public string Adress 
        {
            get { return Driver.Adress; }
            set { Driver.Adress = value; }
        }

        [Display(Name="Telefonnummer")]
        public string PhoneNumber 
        {
            get { return Driver.PhoneNumber; }
            set { Driver.PhoneNumber = value; }
        }
    }

    public class StatisticsSetModel
    {
        public List<StatisticsModel> StatisticsModels { get; set; } = new List<StatisticsModel>();
        public int BookingCount { get; set; }
        public bool IsForDriver { get; set; }

        public string InfoText
            => $"Totalt {StatisticsModels.Count} olika {(IsForDriver ? "förare" : "företag")} och totalt {BookingCount} bokningar.";

        public enum ChartModes
        {
            [Display(Name = "Datum")]
            Date,
            [Display(Name = "Veckodagar")]
            DayOfWeek
        }
        public ChartModes ChartMode { get; set; } = ChartModes.Date;
    }

    public class StatisticsModel
    {
        public List<BookingEvent> BookingEvents { get; set; }
        public string CompanyName { get; set; }
        public int BookingCount { get; set; }
        public int Index { get; set; }
        public User User { get; set; }
        public bool IsForDriver => User != null;

        public string InfoText => $"Totalt {BookingCount} bokningar.";
        public string Name => IsForDriver ? User.FullName : CompanyName;
        public string Number => IsForDriver ? User.PhoneNumber : BookingEvents.First().CustomerOrgNo;
        public string Email => IsForDriver ? User.Email : BookingEvents.First().CustomerEmail;

        public enum ChartModes
        {
            [Display(Name = "Datum")]
            Date,
            [Display(Name = "Veckodagar")]
            DayOfWeek
        }
        public ChartModes ChartMode { get; set; } = ChartModes.Date;

        public StatisticsModel(List<BookingEvent> events)
        {
            BookingEvents = events;
            CompanyName = events.First().CompanyName;
            BookingCount = events.Count;
        }
    }
    public class StatisticsChart
    {
        public string[] Dates { get; set; }
        public string[] EventCount { get; set; }
        public string Header { get; set; }
        public string XAxis { get; set; }
        public DateTime FirstDate { get; set; }
        public DateTime LastDate { get; set; }

        public string GetTitle() => $"{Header} {FirstDate.ToString("yyyy-M-d")} - {LastDate.ToString("yyyy-M-d")}";
    }
}