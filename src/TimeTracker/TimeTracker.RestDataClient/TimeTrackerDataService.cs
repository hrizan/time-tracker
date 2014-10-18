using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeTracker.Backend.Models;
using TimeTracker.Models;

namespace TimeTracker.RestDataClient
{

    

    namespace TimeTracker.ClientRest
    {
        public class TimeTrackerDataService
        {
            public const string AUTH_TOKEN = "AUTH_TOKEN";

            string _AuthKey;

            string _apiUrl;

            public TimeTrackerDataService(string apiUrl)
                : this(apiUrl, string.Empty)
            { }


            public TimeTrackerDataService(string apiUrl, string agentAuthKey)
            {
                _apiUrl = apiUrl;
                _AuthKey = agentAuthKey;
            }

            private void AddCredentialsToRestRequest(RestRequest request)
            {
                if  (!string.IsNullOrEmpty(_AuthKey))
                {
                    request.AddHeader(AUTH_TOKEN, _AuthKey);
                }
            }

            #region Login with device
            public void LoginWithDeviceAsync(LoginModelWithDevice loginModel, Action<IRestResponse<UserProfileWithDeviceDto>> callback)
            {
                RestClient client = new RestClient(_apiUrl);
                var request = PrepareLoginWithDeviceRequest(loginModel);

                client.ExecuteAsync<UserProfileWithDeviceDto>(request, callback);
            }

            public IRestResponse<UserProfileWithDeviceDto> LoginWithDevice(LoginModelWithDevice loginModel)
            {
                RestClient client = new RestClient(_apiUrl);
                var request = PrepareLoginWithDeviceRequest(loginModel);

                return client.Execute<UserProfileWithDeviceDto>(request);
            }

            private RestRequest PrepareLoginWithDeviceRequest(LoginModelWithDevice activity)
            {
                var request = new RestRequest(new Uri("Account/LoginWithDevice", UriKind.Relative), Method.POST);

                request.RequestFormat = DataFormat.Json;
                request.AddBody(activity);
                AddCredentialsToRestRequest(request);

                return request;
            }

            #endregion

            #region RegisterActivities
            public void RegisterActivityAsync(ActivityUpdateDto activity, Action<IRestResponse<ActivityUpdateDto>> callback)
            {
                RestClient client = new RestClient(_apiUrl);
                var request = PrepareRegisterActivityRequest(activity);

                client.ExecuteAsync<ActivityUpdateDto>(request, callback);
            }

            public IRestResponse<ActivityUpdateDto> RegisterActivity(ActivityUpdateDto activity)
            {
                RestClient client = new RestClient(_apiUrl);
                var request = PrepareRegisterActivityRequest(activity);

                return client.Execute<ActivityUpdateDto>(request);
            }

            private RestRequest PrepareRegisterActivityRequest(ActivityUpdateDto activity)
            {
                var request = new RestRequest(new Uri("Activity/PostActivity", UriKind.Relative), Method.POST);

                request.RequestFormat = DataFormat.Json;
                request.AddBody(activity);
                AddCredentialsToRestRequest(request);

                return request;
            }

            #endregion

            //#region RegisterActivitiesList
            //public void RegisterActivityListAsync(List<ActivityUpdateDto> activities, Action<IRestResponse<List<ActivityUpdateDto>>> callback)
            //{
            //    RestClient client = new RestClient(_apiUrl);
            //    var request = PrepareRegisterModuleCheckListRequest(activities);

            //    client.ExecuteAsync<List<ActivityUpdateDto>>(request, callback);
            //}

            //public IRestResponse<List<ActivityUpdateDto>> RegisterActivityList(List<ActivityUpdateDto> activities)
            //{
            //    RestClient client = new RestClient(_apiUrl);
            //    var request = PrepareRegisterModuleCheckListRequest(activities);

            //    return client.Execute<List<ActivityUpdateDto>>(request);
            //}

            //private RestRequest PrepareRegisterModuleCheckListRequest(List<ActivityUpdateDto> moduleChecks)
            //{
            //    var request = new RestRequest(new Uri("Common/RegisterModuleCheckList", UriKind.Relative), Method.POST);

            //    request.RequestFormat = DataFormat.Json;
            //    request.AddBody(moduleChecks);
            //    AddCredentialsToRestRequest(request);

            //    return request;
            //}
            //#endregion

            //#region GetSchedules
            //public void GetSchedulesAsync(string agentGuid, Action<IRestResponse<List<Schedule>>> callback)
            //{
            //    RestClient client = new RestClient(_apiUrl);
            //    var request = PrepareGetSchedulesRequest(agentGuid);

            //    client.ExecuteAsync(request, callback);
            //}

            //public IRestResponse<List<Schedule>> GetSchedules(string agentGuid)
            //{
            //    RestClient client = new RestClient(_apiUrl);
            //    var request = PrepareGetSchedulesRequest(agentGuid);

            //    return client.Execute<List<Schedule>>(request);
            //}

            //private RestRequest PrepareGetSchedulesRequest(string agentGuid)
            //{
            //    string relativeUrl = string.Format("Common/GetSchedules?agentGuid={0}", agentGuid);
            //    var request = new RestRequest(new Uri(relativeUrl, UriKind.Relative), Method.GET);
            //    AddCredentialsToRestRequest(request);
            //    return request;
            //}
            //#endregion

        }
    }

}
