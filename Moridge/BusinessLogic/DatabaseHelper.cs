using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Moridge.Extensions;
using Moridge.Models;

namespace Moridge.BusinessLogic
{
    public class DatabaseHelper
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public DatabaseHelper()
        {
            _dbContext = new ApplicationDbContext();
            _userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_dbContext));
            _userManager.UserValidator = new UserValidator<ApplicationUser>(_userManager) { AllowOnlyAlphanumericUserNames = false };
        }

        /// <summary>
        /// Gets the user based off given ID or for the current web user.
        /// </summary>
        /// <param name="userId">user id or null</param>
        /// <returns><see cref="ApplicationUser"/> or null</returns>
        public ApplicationUser FindUser(string userId = null) => _userManager.FindById(userId ?? HttpContext.Current.User.Identity.Name);

        /// <summary>
        /// Saves database changes.
        /// </summary>
        public void Save() => _dbContext.SaveChanges();

        /// <summary>
        /// Finds user by email and password.
        /// </summary>
        /// <param name="email">user email</param>
        /// <param name="password">user password</param>
        /// <returns><see cref="ApplicationUser"/> or null</returns>
        public ApplicationUser FindUserByEmail(string email, string password) => _userManager.FindByEmail(email, password);

        /// <summary>
        /// Gets a users roles based off given id.
        /// </summary>
        /// <param name="id">user id</param>
        /// <returns>list of roles</returns>
        public IList<string> GetRoles(string id) => _userManager.GetRoles(id);

        /// <summary>
        /// Creates a new user and saves it to the database.
        /// </summary>
        /// <param name="user">user to save</param>
        /// <param name="password">user password</param>
        /// <returns>creation result</returns>
        public IdentityResult CreateUser(ApplicationUser user, string password) => _userManager.Create(user, password);

        /// <summary>
        /// Updates a user with given data.
        /// </summary>
        /// <param name="modelUser">model user containing new data</param>
        /// <returns>update result</returns>
        public IdentityResult UpdateUser(ApplicationUser modelUser)
        {
            //need to read the user from the database to be able to modifiy it
            var user = FindUser(modelUser.Id);

            //apply any modifications
            user.FirstName = modelUser.FirstName;
            user.LastName = modelUser.LastName;
            user.Email = modelUser.Email;
            user.Adress = modelUser.Adress;
            user.PhoneNumber = modelUser.PhoneNumber;
            return _userManager.Update(user);
        }

        /// <summary>
        /// Deletes a user based off given id.
        /// </summary>
        /// <param name="userId">id of user to remove</param>
        /// <returns>result</returns>
        public IdentityResult DeleteUser(string userId)
        {
            var user = FindUser(userId);
            return user == null ? IdentityResult.Failed($"User with id {userId} was not found.") : _userManager.Delete(user);

            //Manually remove references if cascade delete failed.
            foreach (var daySchedule in user.Schedule.ToList())
            {
                user.Schedule.Remove(daySchedule);
            }
            foreach (var scheduleDeviation in user.ScheduleDeviation.ToList())
            {
                user.ScheduleDeviation.Remove(scheduleDeviation);
            }
            _dbContext.SaveChanges();
        }

        /// <summary>
        /// Adds a user to a role.
        /// </summary>
        /// <param name="id">user id</param>
        /// <param name="role">role</param>
        public void AddUserToRole(string id, string role) => _userManager.AddToRole(id, role);

        /// <summary>
        /// Gets the user's role.
        /// </summary>
        /// <param name="id">user id</param>
        /// <returns>user's role</returns>
        public string GetUserRole(string id) => _userManager.GetRoles(id)?.First();

        /// <summary>
        /// Gets all users in the given role.
        /// </summary>
        /// <param name="roleId">id of role to get</param>
        /// <returns>list of users</returns>
        public List<ApplicationUser> FindAllUsersInRole(string roleId)
        {
            return _userManager.Users.ToList()
                .Where(user => user.Roles.Count > 0 && user.Roles.First().RoleId.Equals(roleId))
                .ToList();
        }

    }
}