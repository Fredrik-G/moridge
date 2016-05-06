using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using Microsoft.AspNet.Identity.EntityFramework;
using Moridge.Models;

namespace Moridge.BusinessLogic
{
    /// <summary>
    /// Contains information and methods regarding roles.
    /// </summary>
    public static class RolesHelper
    {
        /// <summary>
        /// Admin role string
        /// </summary>
        public const string ADMIN_ROLE = "Admin";
        /// <summary>
        /// Driver role string
        /// </summary>
        public const string DRIVER_ROLE = "Driver";

        /// <summary>
        /// Sets up the roles used in this application
        /// </summary>
        /// <param name="context">Application's database context</param>
        public static void SetupRoles(ApplicationDbContext context)
        {
            var driverRole = context.Roles.Add(new IdentityRole(DRIVER_ROLE));
            var adminRole = context.Roles.Add(new IdentityRole(ADMIN_ROLE));
        }

        /// <summary>
        /// Gets the next page based on the given user's role.
        /// Values are returned by the out parameters actionName and controllerName.
        /// </summary>
        /// <param name="userManager">Application's database context</param>
        /// <param name="id">user id</param>
        /// <param name="actionName">name for the next page</param>
        /// <param name="controllerName">controller for the next page</param>
        public static void GetPageForUser(IList<string> userActiveRoles, out string actionName, out string controllerName)
        {
            if (userActiveRoles.Count > 1)
            {
                //More than one role = something is wrong. Log it.
                //log.log("User " + id + " has more than one role.");
                //TODO logga
            }

            var userActiveRole = userActiveRoles.First();
            GetPageForUser(null, out actionName, out controllerName, userActiveRole);
        }

        /// <summary>
        /// Gets the next page based on the given user's role.
        /// Values are returned by the out parameters actionName and controllerName.
        /// </summary>
        /// <param name="user">user to process, null if using <paramref name="role"/></param>
        /// <param name="actionName">name for the next page</param>
        /// <param name="controllerName">controller for the next page</param>
        /// <param name="role">user's role. should be skipped if using <paramref name="user"/> </param>
        public static void GetPageForUser(IPrincipal user, out string actionName, out string controllerName, string role = "")
        {
            if (user != null && user.IsInRole(ADMIN_ROLE) ||
                role.Equals(ADMIN_ROLE))
            {
                actionName = "DriverRegister";
                controllerName = ADMIN_ROLE;
            }
            else if (user != null && user.IsInRole(DRIVER_ROLE) ||
                role.Equals(DRIVER_ROLE))
            {
                actionName = "BookingDay";
                controllerName = DRIVER_ROLE;
            }
            //nor admin or driver => error, show login page again.
            else
            {
                actionName = "Login";
                controllerName = "Account";
            }
        }

        public static void AddUserToRole(DatabaseHelper dbHelper, string id, string role)
        {
            //nor admin or driver => error
            if(!(role.Equals(ADMIN_ROLE) || role.Equals(DRIVER_ROLE)))
            {
                //log.log($"Error adding role {role} to userID {id}.");
                //TODO logga
                return;
            }

            dbHelper.AddUserToRole(id, role);
        }
    }
}
