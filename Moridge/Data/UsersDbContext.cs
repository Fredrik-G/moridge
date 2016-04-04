using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Moridge.Data
{
    public class UsersDbContext : IdentityDbContext<IdentityUser>
    {
        static UsersDbContext()
        {
            Database.SetInitializer(new Initializer());
        }

        private class Initializer : CreateDatabaseIfNotExists<UsersDbContext>
        {
            //    protected override void Seed(UsersDbContext context)
            //    {
            //        IdentityRole role = context.Roles.Add(new IdentityRole("User"));

            //        IdentityUser user = new IdentityUser("SampleUser");
            //        user.Claims.Add(new IdentityUserClaim
            //        {
            //            ClaimType = "hasRegistered",
            //            ClaimValue = "true"
            //        });

            //        user.PasswordHash = new PasswordHasher().HashPassword("secret");
            //        context.Users.Add(user);
            //        context.SaveChanges();
            //        base.Seed(context);
            //    }
        }
    }
}
