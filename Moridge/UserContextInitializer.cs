using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Moridge
{
    public class UserContextInitializer : IDatabaseInitializer<UserContext>
    {
        public void InitializeDatabase(UserContext context)
        {
        }
    }
}