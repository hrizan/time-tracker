using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TimeTracker.Backend.Models;

namespace TimeTracker.Backend.Controllers
{
    public class ApiControllerBase : ApiController
    {
        public Guid? CurrentUserConsumerId
        {
            get
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return null;
                }

                CustomIdentity user = (CustomIdentity)User.Identity;
                return user.ConsumerId;
            }
        }

        public string CurrentUsername
        {
            get
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return null;
                }

                return User.Identity.Name;
            }
        }


    }
}