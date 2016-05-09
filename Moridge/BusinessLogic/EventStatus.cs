using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Moridge.Models;

namespace Moridge.BusinessLogic
{
    public class EventStatus
    {
        public enum Status
        {
            [Display(Name = "Ej påbörjad")]
            NotSet,
            [Display(Name = "Fordon upphämtat")]
            VehiclePickedUp,
            [Display(Name = "Fordon tvättas")]
            VehicleAtWash,
            [Display(Name = "Fordon färdigt")]
            VehicleDone,
            [Display(Name = "Fordon levererat")]
            VehicleDelivered,
        }

        /// <summary>
        /// Converts a string to Status enum./>
        /// </summary>
        /// <param name="enumString"></param>
        /// <returns></returns>
        public Status StringToStatus(string enumString) => (Status)Enum.Parse(typeof(Status), enumString);
    }
}