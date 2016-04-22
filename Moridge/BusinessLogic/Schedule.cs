using System;
using System.Collections.Generic;
using System.Linq;
using Google.Apis.Calendar.v3.Data;
using Moridge.Models;

namespace Moridge.BusinessLogic
{
    /// <summary>
    /// Contains information and methods regarding booking.
    /// </summary>
    public class Schedule
    {
        public DaysInfo Days { get; } = new DaysInfo();
        public Occassions Occassions { get; } = new Occassions();
        
    }
}