using System;
using System.ComponentModel.DataAnnotations;

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
        /// Converts a string to Status enum.>
        /// Returns Status.NotSet if string value is not found/defined./>
        /// </summary>
        /// <param name="enumString">status string to convert</param>
        /// <returns>Status value from string or default Status.NotSet</returns>
        public static Status StringToStatus(string enumString)
        {
            Status status;
            Enum.TryParse(enumString, out status);
            return status;
        }
    }
}