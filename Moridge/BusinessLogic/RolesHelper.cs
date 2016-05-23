using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public const string AdminRole = "Admin";
        /// <summary>
        /// Admin's role Id. 
        /// Hardcoded to optimize performance.
        /// </summary>
        public const string AdminRoleId = "1c7f0db3-11ee-45e3-a349-a99aaca8ae41";
        /// <summary>
        /// Driver role string
        /// </summary>
        public const string DriverRole = "Driver";
        /// <summary>
        /// Driver's role Id. 
        /// Hardcoded to optimize performance.
        /// </summary>
        public const string DriverRoleId = "90f3885d-b271-424d-9fdc-79392c60eefc";

        public enum Roles
        {
            [Display(Name = "Förare")]
            Driver,
            [Display(Name = "Administratör")]
            Admin
        }

        /// <summary>
        /// Sets up the roles used in this application
        /// </summary>
        /// <param name="context">Application's database context</param>
        public static void SetupRoles(ApplicationDbContext context)
        {
            var driverRole = context.Roles.Add(new IdentityRole(DriverRole));
            var adminRole = context.Roles.Add(new IdentityRole(AdminRole));
        }

        /// <summary>
        /// Gets the next page based on the given user's role.
        /// Values are returned by the out parameters actionName and controllerName.
        /// </summary>
        /// <param name="userActiveRoles">list of user's roles</param>
        /// <param name="actionName">name for the next page</param>
        /// <param name="controllerName">controller for the next page</param>
        public static void GetPageForUser(IList<string> userActiveRoles, out string actionName, out string controllerName)
        {
            if (userActiveRoles.Count > 1)
            {
                //More than one role = something is wrong. Log it.
                Logger.LogError($"GetPageForUser: user has more than one role {userActiveRoles}.");
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
            if (user != null && user.IsInRole(AdminRole) ||
                role.Equals(AdminRole))
            {
                actionName = "DriverRegistry";
                controllerName = AdminRole;
            }
            else if (user != null && user.IsInRole(DriverRole) ||
                role.Equals(DriverRole))
            {
                actionName = "BookingDay";
                controllerName = DriverRole;
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
            if(!(role.Equals(AdminRole) || role.Equals(DriverRole)))
            {
                Logger.LogError($"AddUserToRole: Error adding role {role} to userID {id}.");
                return;
            }

            dbHelper.AddUserToRole(id, role);
        }
    }
}
