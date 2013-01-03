using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.Mvc;

namespace iBoox.Models
{
    public class AccountsListViewModel
    {
        public string SearchInput { get; set; }
        public SelectList SearchOptionList { get; set; }

        public MembershipUserCollection Accounts { get; set; }

        public string UsersOnlineNow
        {
            get { return Membership.GetNumberOfUsersOnline().ToString(); } 
        }

        public string RegisteredUsers {
            get { return Membership.GetAllUsers().Count.ToString(); }
        }
    }
}