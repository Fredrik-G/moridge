using Google.Apis.Calendar.v3.Data;

namespace Moridge.BusinessLogic
{
    /// <summary>
    /// Contains information about occassions.
    /// </summary>
    public class Occassions
    {
        public string[] OccasionsPerDay => new[] { "Förmiddag", "Eftermiddag" };
        public string CurrentOccassion { get; set; }
        public Events EventsThisOccassion { get; set; }
    }
}