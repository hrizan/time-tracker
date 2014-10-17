using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using WebMatrix.WebData;

namespace TimeTracker.Backend.Models
{
    public class WebSecurityHelpers
    {
        public static void DeleteUser(string userName)
        {
            if (Roles.GetRolesForUser(userName).Count() > 0)
            {
                Roles.RemoveUserFromRoles(userName, Roles.GetRolesForUser(userName));
            }

            ((SimpleMembershipProvider)Membership.Provider).DeleteAccount(userName); // deletes record from webpages_Membership table
            ((SimpleMembershipProvider)Membership.Provider).DeleteUser(userName, true); // deletes record from UserProfile table

        }
    }
}