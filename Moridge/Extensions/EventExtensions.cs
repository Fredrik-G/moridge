using Google.Apis.Calendar.v3.Data;
using MyMoridgeServer.BusinessLogic;

namespace Moridge.Extensions
{
    public static class EventExtensions
    {
        /// <summary>
        /// Gets vehicle reg no for event.
        /// </summary>
        /// <param name="bookingEvent">event to use</param>
        /// <returns>vehicle reg no or empty string</returns>
        public static string GetVehicleRegNo(this Event bookingEvent)
        {
            var summary = bookingEvent.Summary.Split('-');
            return summary?[1] ?? string.Empty;
        }
    }
}