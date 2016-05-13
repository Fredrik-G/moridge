using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Moridge.BusinessLogic;
using MyMoridgeServer.Models;

namespace Moridge.Models
{
    public class DriverRegisterModel
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
    }
    public class StatisticsModel
    {
        public List<BookingEvent> BookingEvents { get; set; }
        public string CompanyName { get; set; }
        public int NumberOfEvents { get; set; }
        public string Debug { get; set; }
        public int Index { get; set; }

        public StatisticsModel(List<BookingEvent> events)
        {
            BookingEvents = events;
            CompanyName = events.First().CompanyName;
            NumberOfEvents = events.Count;
        }
    }
    public class StatisticsChart
    {
        public string[] Dates { get; set; }
        public string[] EventCount { get; set; }
        public string CompanyName { get; set; }
        public DateTime FirstDate { get; set; }
        public DateTime LastDate { get; set; }

        public string GetTitle() => $"{CompanyName} {FirstDate.ToString("yyyy-M-d")} - {LastDate.ToString("yyyy-M-d")}";
    }
}