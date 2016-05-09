using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Moridge.BusinessLogic
{
    public class User
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
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

            FirstName = user.FirstName;
            LastName = user.LastName;
            Email = user.Email;
            Adress = user.Adress;
            PhoneNumber = user.PhoneNumber;
        }
    }

    public static class UserHelper
    {
        /// <summary>
        /// Saves the current web user to the session storage.
        /// </summary>
        /// <param name="userId">user id or null</param>
        public static void SaveUserToSession(string userId = null)
        {
            var user = new User();
            user.GetUserFromDatabase(userId);
            System.Web.HttpContext.Current.Session["CurrentUser"] = user;
        }
    }
}