using System.Web;

namespace Moridge.BusinessLogic
{
    public class Logger
    {
        public static void LogError(string message)
        {
            if (!HttpContext.Current.IsDebuggingEnabled)
            {
                MyMoridgeServer.BusinessLogic.Common.LogError(message, HttpContext.Current.User.Identity.Name);
            }
        }
    }
}