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
            [Display(Name = "Service utförs")]
            ServiceStarted,
            [Display(Name = "Service färdig")]
            ServiceDone,
            [Display(Name = "Fordon levererat")]
            VehicleDelivered
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
            var splitted = enumString.Split('-');
            if (splitted.Length == 3)
            {
                Enum.TryParse(splitted[2], out status);
            }
            else
            {
                Enum.TryParse(enumString, out status);
            }
            return status;
        }

        /// <summary>
        /// Checks if the next/previous status is valid.
        /// </summary>
        /// <param name="status">current status</param>
        /// <param name="isNextStatus">next or previous status</param>
        /// <returns>true if valid, otherwise false</returns>
        public static bool IsStatusValid(Status status, bool isNextStatus)
        {
            //handle first/last status separately
            switch (status)
            {
                case Status.NotSet:
                    return isNextStatus;
                case Status.VehicleDelivered:
                    return !isNextStatus;
            }
            //any other status is always valid, return true
            return true;
        }
    }
}