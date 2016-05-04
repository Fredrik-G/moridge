using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using Moridge.BusinessLogic;

namespace Moridge.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Adress { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public virtual ICollection<DaySchedule> Schedule { get; set; }
        public virtual ICollection<ScheduleDeviation> ScheduleDeviation { get; set; }
    }

    /// <summary>
    /// Contains a drivers daily schedule and the number of bookings.
    /// </summary>
    public class DaySchedule
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string DayOfWeek { get; set; }
        public int Morning { get; set; }
        public int Afternoon { get; set; }
        public bool MorningActive { get; set; }
        public bool AfternoonActive { get; set; }
    }

    /// <summary>
    /// Contains any deviations from the normal day-to-day schedule.
    /// </summary>
    public class ScheduleDeviation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime? Date { get; set; }
        public string DayOfWeek { get; set; }
        public int? Morning { get; set; }
        public int? Afternoon { get; set; }
        public bool? MorningActive { get; set; }
        public bool? AfternoonActive { get; set; }
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