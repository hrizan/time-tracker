using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TimeTracker.Backend.Models
{
    public class AccountConstants
    {
        public const string USER_ROLE_COMPANY = "Company";
        public const string USER_ROLE_ADMIN = "Admin";
        public const string USER_ROLE_CONSUMER = "Consumer";


        public const string USER_ROLE_ADMIN_AND_COMPANY = "Company, Admin";
    }
}