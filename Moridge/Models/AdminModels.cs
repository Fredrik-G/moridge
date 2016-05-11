using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Moridge.Models
{
    public class DriverRegisterModel
    {
        public List<ApplicationUser> Drivers { get; set; } 
    }

    public class DriverDetailsModel
    {
        public ApplicationUser Driver { get; set; }
         
        [Display(Name="Förnamn")]
        public string FirstName => Driver.FirstName;

        [Display(Name="Efternamn")]
        public string LastName => Driver.LastName;

        [Display(Name="E-post")]
        public string Email => Driver.Email;

        [Display(Name="Adress")]
        public string Adress => Driver.Adress;

        [Display(Name="Telefonnummer")]
        public string PhoneNumber => Driver.PhoneNumber;
    }
}