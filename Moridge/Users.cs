//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Moridge
{
    using System;
    using System.Collections.Generic;

    public partial class User : IdentityUser  //IUser<string>
    {
      //  public string Id { get; set; }
      //  public string UserName { get { return Email; } set { Email = value; } }
        public string FirstName { get; set; }
        public string LastName { get; set; }
   //     public string Email { get; set; }
        public string Password { get; set; }
    }
}
