using Moridge.BusinessLogic;

namespace Moridge.Models
{
    public class SidePanelLoggedInModel
    {
        public SidePanelUser User { get; set; }
    }

    public class SidePanelUser
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Adress { get; set; }
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Gets the web user's information from the database
        /// and saves user info for this sidepaneluser.
        /// </summary>
        /// <param name="userId">user id or null</param>
        public void GetUserFromDatabase(string userId)
        {
            var dbHelper = new DatabaseHelper();
            var user = dbHelper.FindUser(userId);

            Name = user.UserName;//TODO add name to db
            Email = user.Email;
            Adress = user.Id;//TODO add adress to db
            PhoneNumber = user.PhoneNumber;
        }
    }

    public static class SidePanelHelper
    {
        /// <summary>
        /// Saves the current web user to the session storage.
        /// </summary>
        /// <param name="userId">user id or null</param>
        public static void SaveUserToSession(string userId = null)
        {
            var sidePanelUser = new SidePanelUser();
            sidePanelUser.GetUserFromDatabase(userId);
            System.Web.HttpContext.Current.Session["SidePanelUser"] = sidePanelUser;
        }
    }
}