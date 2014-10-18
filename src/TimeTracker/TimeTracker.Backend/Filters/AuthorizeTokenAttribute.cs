using TimeTracker.Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Security;


namespace TimeTracker.Backend.Filters
{
    public class AuthorizeTokenAttribute : AuthorizationFilterAttribute
    {
        public const string TOKEN_HEADER = "AUTH_TOKEN";

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (actionContext != null)
            {
                if (!AuthorizeRequest(actionContext.ControllerContext.Request))
                {
                    actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized) { RequestMessage = actionContext.ControllerContext.Request };
                }
                return;
            }
        }

        private bool AuthorizeRequest(System.Net.Http.HttpRequestMessage request)
        {
            if (request.Headers.Contains(TOKEN_HEADER))
            {
                var tokenValue = request.Headers.GetValues(TOKEN_HEADER);
                if (tokenValue.Count() == 1)
                {
                    var authToken = tokenValue.FirstOrDefault();
                    if (authToken == null)
                    {
                        return false;
                    }

                    //Token validation logic here
                    var value = tokenValue.FirstOrDefault();
                    var usersDb = new UsersContext();
                    var userProfile = usersDb.UserProfiles.Single(u => u.AuthToken == value);

                    if (userProfile != null)
                    {
                        var userIdentity = new CustomIdentity(userProfile.UserName, userProfile.ConsumerId);
                        
                        var principal = new GenericPrincipal(userIdentity, Roles.GetRolesForUser(userProfile.UserName));
                        Thread.CurrentPrincipal = principal;
                        HttpContext.Current.User = principal;
                        
                        return true;
                    }


                }
            }
            //#if DEBUG
            //            else
            //            {
            //                UsersContext users = new UsersContext();
            //                var user = users.UserProfiles.Where(up => up.UserName.ToUpper() == "demo".ToUpper()).FirstOrDefault();

            //                if (user == null)
            //                {
            //                    return false;
            //                }

            //                IPrincipal principal = new GenericPrincipal(new GenericIdentity(user.UserName), new string[] { });
            //                //set current principal
            //                Thread.CurrentPrincipal = principal;
            //                HttpContext.Current.User = principal;

            //                authorized = true;

            //            }
            //#endif
            return false;
        }
    }
}