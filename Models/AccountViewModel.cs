using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace iBoox.Models
{
    public class AccountViewModel
    {
        public MembershipUser Account { get; set; }
        public string[] AccountRoles
        {
            get { return Roles.GetAllRoles(); } 
        }
    }
}