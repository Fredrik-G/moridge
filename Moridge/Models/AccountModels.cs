using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Moridge;

namespace Moridge.Models
{
    public class ChangePasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class CustomUserManager : UserManager<User>
    {
        public CustomUserManager(IUserStore<User> store)
            : base(store)
        {
        }
    }
    public class CustomSignInManager : SignInManager<User, string>
    {
        public CustomSignInManager(CustomUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }
    }
    //public class CustomUser : IUser<string>
    //{
    //    public string Id { get; set; }

    //    public string UserName { get; set; }
    //}

   /* public class CustomAuthentication : IAuthenticationManager
    {

        public Task<IEnumerable<AuthenticateResult>> AuthenticateAsync(string[] authenticationTypes)
        {
            throw new NotImplementedException();
        }

        public Task<AuthenticateResult> AuthenticateAsync(string authenticationType)
        {
            throw new NotImplementedException();
        }

        public AuthenticationResponseChallenge AuthenticationResponseChallenge
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public AuthenticationResponseGrant AuthenticationResponseGrant
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public AuthenticationResponseRevoke AuthenticationResponseRevoke
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void Challenge(params string[] authenticationTypes)
        {
            throw new NotImplementedException();
        }

        public void Challenge(AuthenticationProperties properties, params string[] authenticationTypes)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<AuthenticationDescription> GetAuthenticationTypes(Func<AuthenticationDescription, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<AuthenticationDescription> GetAuthenticationTypes()
        {
            throw new NotImplementedException();
        }

        public void SignIn(params System.Security.Claims.ClaimsIdentity[] identities)
        {
            throw new NotImplementedException();
        }

        public void SignIn(AuthenticationProperties properties, params System.Security.Claims.ClaimsIdentity[] identities)
        {
            throw new NotImplementedException();
        }

        public void SignOut(params string[] authenticationTypes)
        {
            throw new NotImplementedException();
        }

        public System.Security.Claims.ClaimsPrincipal User
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }

    public class CustomUserStore : IUserStore<User>
    {
        private readonly UserContext database;

        public CustomUserStore()
        {
            this.database = new UserContext();
        }

        public void Dispose()
        {
            this.database.Dispose();
        }

        public Task CreateAsync(User user)
        {
            // TODO
            throw new NotImplementedException();
        }

        public Task UpdateAsync(User user)
        {
            // TODO
            throw new NotImplementedException();
        }

        public Task DeleteAsync(User user)
        {
            // TODO
            throw new NotImplementedException();
        }

        public async Task<User> FindByIdAsync(string userId)
        {
            var user = await database.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();
            return user;
        }

        public async Task<User> FindByNameAsync(string userName)
        {
            var user = await database.Users.Where(u => u.UserName == userName).FirstOrDefaultAsync();
            return user;
        }
    }
    * */

    public class RegisterModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
