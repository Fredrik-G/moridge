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
        /// Adds a user to a role.
        /// </summary>
        /// <param name="id">user id</param>
        /// <param name="role">role</param>
        public void AddUserToRole(string id, string role) => _userManager.AddToRole(id, role);

        /// <summary>
        /// Gets all users in the given role.
        /// </summary>
        /// <param name="role">role to get</param>
        /// <returns>list of users</returns>
        public List<ApplicationUser> FindAllUsersInRole(string role)
        {
            return _userManager.Users.ToList()
                .Where(x => _userManager.IsInRole(x.Id, RolesHelper.DRIVER_ROLE))
                .ToList();
        }
    }
}