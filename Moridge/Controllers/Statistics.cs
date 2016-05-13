using System.Collections.Generic;
using System.Linq;
using MyMoridgeServer.Models;

namespace Moridge.Controllers
{
    public class Statistics
    {
        public List<List<BookingEvent>> ReadBookingEvents()
        {
            var bookings = new MyMoridgeServer.BusinessLogic.Booking().GetAllBookings();
            var groupedBookings = bookings.GroupBy(x => x.CustomerOrgNo)
                .Select(group => group.ToList())
                .OrderByDescending(events => events.Count)
                .ToList();

            return groupedBookings;
        }
    }
}