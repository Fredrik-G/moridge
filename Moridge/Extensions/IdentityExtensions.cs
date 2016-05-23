using Microsoft.AspNet.Identity;
using Moridge.Models;

namespace Moridge.Extensions
{
    /// <summary>
    /// Contains extension methods for Identity.
    /// </summary>
    public static class IdentityExtensions
    {
        /// <summary>
        /// Returns a user with the specified email or null if there is no match.
        /// </summary>
        /// <param name="userManager">this user manager to use</param>
        /// <param name="email">user email</param>
        /// <param name="password">user password</param>
        /// <returns></returns>
        public static ApplicationUser FindByEmail
            (this UserManager<ApplicationUser> userManager, string email, string password)
        {
            if (!email.Contains("@")) return null;
            var user = userManager.FindByEmail(email);
            return user != null ? userManager.Find(user.UserName, password) : null;
        }
    }
}