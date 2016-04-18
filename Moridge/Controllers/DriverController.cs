using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using Google.Apis.Calendar.v3.Data;
using Moridge.Helpers;
using Moridge.Models;
using MyMoridgeServer.BusinessLogic;

namespace Moridge.Controllers
{
    [Authorize(Roles = RolesHelper.DRIVER_ROLE + "," + RolesHelper.ADMIN_ROLE)]
    public class DriverController : Controller
    {
        public ActionResult Booking()
        {
            var calendar = new GoogleCalendar(Common.GetAppConfigValue("MoridgeOrganizerCalendarEmail"), Common.GetAppConfigValue("MoridgeMainCalendarEmail"));
            var events = calendar.GetEventList();
            var bookingModel = new BookingModel { Events = events };
            return View(bookingModel);
        }

        public ActionResult Schedule()
        {
            return View();
        }
        public ActionResult PersonalInfo()
        {
            return View();
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
