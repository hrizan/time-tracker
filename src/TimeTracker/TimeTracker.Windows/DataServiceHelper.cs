using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeTracker.Windows.Models;

namespace TimeTracker.Windows
{
    public class DataServiceHelper
    {
        public const string APPLICATION_ID_HEADER = "APPLICATION";

        Guid? _applicationId;

        string _apiUrl;

        public DataServiceHelper(string apiUrl)
            : this(apiUrl, new Guid())
        { }

        /// <summary>
        /// Constructor with parameters
        /// </summary>
        /// <param name="apiUrl">Url of the AcsControl Server Api</param>
        /// <param name="applicationId">Guid retrieved for this agent</param>
        public DataServiceHelper(string apiUrl, Guid applicationId)
        {
            _apiUrl = apiUrl;
            _applicationId = applicationId;
        }

        private void AddCredentialsToRestRequest(RestRequest request)
        {
            if (_applicationId != null)
            {
                request.AddHeader(APPLICATION_ID_HEADER, _applicationId.ToString());
            }
        }

        public string LogIn(LogInModel model)
        {
            RestClient client = new RestClient(_apiUrl);

            var request = PrepareLogInRequest(model);
            
            return client.Execute<Guid>(request).Content;
        }
        private RestRequest PrepareLogInRequest(LogInModel LoginWithDevice)
        {
            var request = new RestRequest(new Uri("Account/LoginWithDevice", UriKind.Relative), Method.POST);

            request.RequestFormat = DataFormat.Json;
            request.AddBody(LoginWithDevice);
            AddCredentialsToRestRequest(request);

            return request;
        }

        public void SendActivity()
        {
 
        }
    }
}
