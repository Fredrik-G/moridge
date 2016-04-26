using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Google.Apis.Calendar.v3.Data;

namespace Moridge.BusinessLogic
{
    /// <summary>
    /// Contains information about one occassion. (morning/afternoon).
    /// </summary>
    public class Occassion
    {
        public string Name { get; set; }

        /// <summary>
        /// The number of bookings a driver is supposed to do.
        /// </summary>
        public int BookingsForDriver { get; set; }

        /// <summary>
        /// The actual number of bookings.
        /// </summary>
        public int NumberOfBookings { get; set; }

        /// <summary>
        /// Determines if this occassion is active.
        /// </summary>
        public bool IsActive { get; set; }
    }
}