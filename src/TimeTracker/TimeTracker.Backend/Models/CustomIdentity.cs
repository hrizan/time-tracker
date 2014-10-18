using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace TimeTracker.Backend.Models
{
    public class CustomIdentity : GenericIdentity
    {
        public Guid? ConsumerId { get; private set; }

        public CustomIdentity(string username, Guid? consumerId):base(username)
        {
            ConsumerId = consumerId;
        }
    }
}