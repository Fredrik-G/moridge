using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Moridge.Models;

namespace Moridge.BusinessLogic
{
    /// <summary>
    /// Contains extension methods for Identity.
    /// </summary>
    public static class IdentityExtensions
    {
        /// <summary>
        /// Returns a user with the specified username or email or null if there is no match.
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="usernameOrEmail"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static ApplicationUser FindByNameOrEmail
            (this UserManager<ApplicationUser> userManager, string usernameOrEmail, string password)
        {
            var username = usernameOrEmail;
            if (usernameOrEmail.Contains("@"))
            {
                var userForEmail = userManager.FindByEmail(usernameOrEmail);
                if (userForEmail != null)
                {
                    username = userForEmail.UserName;
                }
            }
            return userManager.Find(username, password);
        }
    }
}