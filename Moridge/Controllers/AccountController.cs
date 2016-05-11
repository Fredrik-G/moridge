using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Moridge.Models;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Moridge.BusinessLogic;
using Moridge.Extensions;

namespace Moridge.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {

        //
        // GET: /Account/Index

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Account/Login

        [AllowAnonymous]
        public ActionResult Login()
        {
            return View(new LoginModel());
        }

        //
        // POST: /Account/Login

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (!ModelState.IsValid) return View(model);

            var dbHelper = new DatabaseHelper();
            var user = dbHelper.FindUserByEmail(model.Email, model.Password);

            //user was found => correct login
            if (user != null)
            {
                //Gets the next page based off the user's role.
                string actionName, controllerName;
                RolesHelper.GetPageForUser(dbHelper.GetRoles(user.Id), out actionName, out controllerName);
                UserHelper.SaveUserToSession(user.Id);

                //Create authentication cookie with user role.
                CreateAuthenticationCookie(user.Id, controllerName, model.RememberMe);

                return RedirectToAction(actionName, controllerName);
            }
            //incorrect login.
            ModelState.AddModelError("", "The user name or password provided is incorrect.");
            return View(model);
        }

        /// <summary>
        /// Creates an authentication cookie for the logged on user.
        /// </summary>
        /// <param name="userId">user's id</param>
        /// <param name="role">user's role.</param>
        /// <param name="isPersistent">indicates whether the cookie should be kept beyond the current session.</param>
        private void CreateAuthenticationCookie(string userId, string role, bool isPersistent)
        {
            var authTicket = new FormsAuthenticationTicket(1, userId, DateTime.Now,
                DateTime.Now.AddMinutes(60), //TODO DEBUG kör 10min
                isPersistent, role, "/");
            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(authTicket));
            Response.Cookies.Add(cookie);
        }

        //
        // GET: /Account/LogOff

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Account");
        }

        //
        // GET: /Account/Register

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var dbHelper = new DatabaseHelper();
            var user = new ApplicationUser { UserName = model.UserName, Email = model.Email };
            var creationResult = dbHelper.CreateUser(user, model.Password);

            if (creationResult.Succeeded)
            {
                //assign user a role and a default schedule
                RolesHelper.AddUserToRole(dbHelper, user.Id, RolesHelper.DriverRole);
                var schedule = new Schedule(user.Id, dbHelper);
                schedule.CreateDefaultSchedule();

                FormsAuthentication.SetAuthCookie(model.UserName, createPersistentCookie: false);
                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError("", "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.");

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePassword

        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Account/ChangePassword

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {

                // ChangePassword will throw an exception rather
                // than return false in certain failure scenarios.
                bool changePasswordSucceeded;
                try
                {
                    MembershipUser currentUser = Membership.GetUser(User.Identity.Name, userIsOnline: true);
                    changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePasswordSuccess

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
