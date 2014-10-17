using TimeTracker.Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using WebMatrix.WebData;

namespace TimeTracker.Backend.Filters
{
    public class MobileAuthenticationMessageHandler : DelegatingHandler
    {
        public MobileAuthenticationMessageHandler()
        {

        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                                                               CancellationToken cancellationToken)
        {
            Authenticate(request);
            return base.SendAsync(request, cancellationToken);
        }

        protected virtual void Authenticate(HttpRequestMessage request)
        {
            bool authorized = false;
            if (request.Headers.Contains(AuthConstants.AUTH_TOKEN_NAME))
            {
                var tokenValue = request.Headers.GetValues(AuthConstants.AUTH_TOKEN_NAME);
                if (tokenValue.Count() == 1)
                {
                    var value = tokenValue.FirstOrDefault();
                    var usersDb = new UsersContext();
                    var userProfile = usersDb.UserProfiles.Single(u => u.AuthToken == value);
                    if (userProfile != null)
                    {
                        var userIdentity = new GenericIdentity(userProfile.UserName);
                        var principal = new GenericPrincipal(userIdentity, Roles.GetRolesForUser(userProfile.UserName));
                        Thread.CurrentPrincipal = principal;
                        HttpContext.Current.User = principal;
                    }
                }
            }
        }
    }

    public static class AuthConstants
    {
        public const string AUTH_TOKEN_NAME = "AUTH_TOKEN";
    }
}