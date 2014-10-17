using TimeTracker.Backend.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Security;
using TimeTracker.Backend.Models;
using WebMatrix.WebData;

namespace TimeTracker.Backend.Controllers
{
    //[InitializeSimpleMembership]
    public class AccountController : ApiController
    {
        private TimeTrackerContext db = new TimeTrackerContext();
        private UsersContext usersDb = new UsersContext();

        [HttpPost]
        public UserProfile Login(LoginModel model)
        {
            string username = model.UserName;
            string password = model.Password;

            UserProfile loggedUserInfo = LoginUserByUsernameAndPassword(username, password);
            if (loggedUserInfo != null)
            {
                return loggedUserInfo;
            }

            throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.Unauthorized));
        }

        [HttpPost]
        public UserProfile LoginWithDevice(LoginModelWithDevice model)
        {
            throw new NotImplementedException();

            string username = model.UserName;
            string password = model.Password;

            UserProfile loggedUserInfo = LoginUserByUsernameAndPassword(username, password);
            if (loggedUserInfo != null)
            {
                return loggedUserInfo;
            }

            throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.Unauthorized));
        }


        private static UserProfile LoginUserByUsernameAndPassword(string username, string password)
        {
            UserProfile loggedUserInfo = null;
            bool loggedin = WebSecurity.Login(username, password);
            if (loggedin)
            {
                var usersDb = new UsersContext();
                var userProfile = usersDb.UserProfiles.Single(u => u.UserName == username);
                if (string.IsNullOrEmpty(userProfile.AuthToken))
                {
                    userProfile.AuthToken = Guid.NewGuid().ToString("N");
                    usersDb.SaveChanges();
                }

                loggedUserInfo = userProfile;
            }
            return loggedUserInfo;
        }

        [HttpPost]
        public HttpResponseMessage Register(RegisterModelCustomer model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                try
                {
                    WebSecurity.CreateUserAndAccount(model.UserName, model.Password);
                    if (!Roles.RoleExists(AccountConstants.USER_ROLE_CONSUMER))
                    {
                        Roles.CreateRole(AccountConstants.USER_ROLE_CONSUMER);
                    }
                    Roles.AddUserToRole(model.UserName, AccountConstants.USER_ROLE_CONSUMER);

                    //create and assign company
                    var user = usersDb.UserProfiles.Where(u => u.UserName == model.UserName).FirstOrDefault();

                    var consumer = new Consumer()
                    {
                        Name = model.UserName,
                        Email = model.UserName,
                        Username = model.UserName,
                        Created = DateTime.Now,
                        Country = model.Country,
                        //BirthDate = (!string.IsNullOrEmpty(model.BirthDate)
                        //                ? DateTime.ParseExact(model.BirthDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture)
                        //                : (DateTime?)null),
                        Sex = model.Sex
                    };

                    db.Consumers.Add(consumer);
                    
                    db.SaveChanges();

                    //save user company
                    usersDb.SaveChanges();
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created);
                    return response;
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                }
                catch (Exception e)
                {
                    try
                    {
                        if (WebSecurity.UserExists(model.UserName))
                        {
                            WebSecurityHelpers.DeleteUser(model.UserName);
                        }
                    }
                    catch (Exception)
                    {

                        // throw;
                    }
                }

            }

            // If we got this far, something failed
            return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
        }

        public static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
    }
}
