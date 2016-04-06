using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Moridge.Controllers;
using Moridge.Helpers;

namespace Moridge.Models
{
    public class ApplicationUser : IdentityUser
    {
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection")
        {
            Database.SetInitializer(new Initializer());
        }

        private class Initializer : CreateDatabaseIfNotExists<ApplicationDbContext>
        {
            protected override void Seed(ApplicationDbContext context)
            {
               // IdentityUser user = new IdentityUser("SampleUser");
               // user.Claims.Add(new IdentityUserClaim
               // {
               //     ClaimType = "hasRegistered",
               //     ClaimValue = "true"
               // });

               // user.PasswordHash = new PasswordHasher().HashPassword("secret");
               // context.Users.Add(user);

                RolesHelper.SetupRoles(context);
                context.SaveChanges();
                base.Seed(context);
            }
        }
    }
}