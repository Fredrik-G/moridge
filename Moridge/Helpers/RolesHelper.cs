using System;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Moridge.Models;

namespace Moridge.Helpers
{
    /// <summary>
    /// Contains information and methods regarding roles.
    /// </summary>
    public static class RolesHelper
    {
        /// <summary>
        /// Admin role string
        /// </summary>
        public static readonly string ADMIN_ROLE = "Admin";
        /// <summary>
        /// Driver role string
        /// </summary>
        public static readonly string DRIVER_ROLE = "Driver";

        /// <summary>
        /// Sets up the roles used in this application
        /// </summary>
        /// <param name="context">Application's database context</param>
        public static void SetupRoles(ApplicationDbContext context)
        {
            var driverRole = context.Roles.Add(new IdentityRole("Driver"));
            var adminRole = context.Roles.Add(new IdentityRole("Admin"));
        }

        /// <summary>
        /// Gets the next page based on the given user's role.
        /// Values are given by the out parameters actionName and controllerName.
        /// </summary>
        /// <param name="userManager">Application's database context</param>
        /// <param name="id">user id</param>
        /// <param name="actionName">name for the next page</param>
        /// <param name="controllerName">controller for the next page</param>
        public static void GetPageForUser(UserManager<ApplicationUser> userManager, string id, out string actionName, out string controllerName)
        {
            var userActiveRoles = userManager.GetRoles(id);
            if (userActiveRoles.Count() > 1)
            {
                //More than one role = something is wrong. Log it.
                //log.log("User " + id + " has more than one role.");
            }

            var userActiveRole = userActiveRoles.First();
            if (userActiveRole.Equals(ADMIN_ROLE))
            {
                actionName = "Index";
                controllerName = ADMIN_ROLE;
            }
            else if (userActiveRole.Equals(DRIVER_ROLE))
            {
                actionName = "Index";
                controllerName = DRIVER_ROLE;
            }
            //nor admin or driver => error, show login page again.
            else
            {
                actionName = "Login";
                controllerName = "Account";
            }
        }
    }
}