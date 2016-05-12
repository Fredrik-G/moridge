using Moridge.BusinessLogic;

namespace Moridge.Models
{
    public class SidePanelLoggedInModel
    {
        public User User { get; set; }

        public bool UserIsDriver() => User.Role.Equals(RolesHelper.DriverRole);
    }
}