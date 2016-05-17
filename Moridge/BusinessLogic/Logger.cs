using System.Web;

namespace Moridge.BusinessLogic
{
    public class Logger
    {
        /// <summary>
        /// Adds the given error to the database errorlog.
        /// Also adds the current user's ID to the message.
        /// </summary>
        /// <param name="message">error message</param>
        public static void LogError(string message)
        {
            if (!HttpContext.Current.IsDebuggingEnabled)
            {
                MyMoridgeServer.BusinessLogic.Common.LogError(message, HttpContext.Current.User.Identity.Name);
            }
        }
    }
}