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
        /// Returns a user with the specified email or null if there is no match.
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static ApplicationUser FindByEmail
            (this UserManager<ApplicationUser> userManager, string email, string password)
        {
            if (!email.Contains("@")) return null;
            var user = userManager.FindByEmail(email);
            return userManager.Find(user.UserName, password);
        }
    }
}