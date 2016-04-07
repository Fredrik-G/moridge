using System.ComponentModel.DataAnnotations;

namespace Moridge.Models
{
    public class DriverModel
    {
        public string[] Days = { "Måndag", "Tisdag" };

        [Display(Name = "vad är detta")]
        public string Test = "testasd";
    }
}
