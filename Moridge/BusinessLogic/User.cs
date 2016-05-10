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
        public static User SaveUserToSession(string userId = null)
        {
            var user = new User();
            user.GetUserFromDatabase(userId);
            System.Web.HttpContext.Current.Session["CurrentUser"] = user;
            return user;
        }

        /// <summary>
        /// Gets the current logged in user from the session store or from the database.
        /// </summary>
        /// <returns></returns>
        public static User GetCurrentUser()
        {
            //The session may be null if the web user access the page "in the wrong way", ie debugging
            //in that case, re-assign session user. 
            return System.Web.HttpContext.Current.Session["CurrentUser"] as User ?? UserHelper.SaveUserToSession();
        }
    }
    
}