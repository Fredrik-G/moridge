using System.Web;
using Moridge.Models;

namespace Moridge.BusinessLogic
{
    public class User
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Adress { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        /// <summary>
        /// Gets the web user's information from the database
        /// and saves user info for this sidepaneluser.
        /// </summary>
        /// <param name="userId">user id or null</param>
        /// <param name="email">email or null</param>
        /// <returns>true if successfully found user, otherwise false</returns>
        public bool GetUserFromDatabase(string userId = null, string email = null)
        {
            var dbHelper = new DatabaseHelper();
            ApplicationUser user;
            if (email != null)
            {
                user = dbHelper.FindUserByEmail(email);
                //user wasn't found => return false
                if (user == null)
                {
                    return false;
                }
            }
            else
            {
                userId = userId ?? HttpContext.Current.User.Identity.Name;
                user = dbHelper.FindUser(userId);
                Role = dbHelper.GetUserRole(userId);
            }
            FirstName = user.FirstName;
            LastName = user.LastName;
            Email = user.Email;
            Adress = user.Adress;
            PhoneNumber = user.PhoneNumber;
            return true;
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

        public static User GetUserByEmail(string email)
        {
            var user = new User();
            var userFound = user.GetUserFromDatabase(email: email);
            return userFound ? user : null;
        }
    }
    
}