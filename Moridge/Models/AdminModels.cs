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
        public ApplicationUser Driver { get; set; } = new ApplicationUser();

        [Display(Name = "Förnamn")]
        public string FirstName
        {
            get { return Driver.FirstName; }
            set { Driver.FirstName = value; }
        }

        [Display(Name="Efternamn")]
        public string LastName
        {
            get { return Driver.LastName; }
            set { Driver.LastName = value; }
        }

        [Display(Name="E-post")]
        public string Email 
        {
            get { return Driver.Email; }
            set
            {
                Driver.Email = value;
                Driver.UserName = value;
            }
        }

        [Display(Name="Adress")]
        public string Adress 
        {
            get { return Driver.Adress; }
            set { Driver.Adress = value; }
        }

        [Display(Name="Telefonnummer")]
        public string PhoneNumber 
        {
            get { return Driver.PhoneNumber; }
            set { Driver.PhoneNumber = value; }
        }
    }
}