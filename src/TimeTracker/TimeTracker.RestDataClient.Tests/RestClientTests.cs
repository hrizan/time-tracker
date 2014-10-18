using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TimeTracker.RestDataClient.TimeTracker.ClientRest;
using TimeTracker.Models;
using TimeTracker.Backend.Models;
using RestSharp;
using System.Diagnostics;

namespace TimeTracker.RestDataClient.Tests
{
    [TestClass]
    public class RestClientTests
    {
        [TestMethod]
        public void Test_Register_Activity()
        {
            string apiUrl = "";
            string authKey = "";
            Guid deviceId = new Guid();//trqbva da se zaredi ot nastroikite
            string computerName = "DOBI_PC";
            TimeTrackerDataService dataService = new TimeTrackerDataService(apiUrl, authKey);
            
            string processName =  "Chrome.exe";
            string resourceName =  "Facebook.com";
            string resourceDescription = "Facebook - Momchil hardalov profile";//process title
            DateTime processUsageFrom = DateTime.UtcNow.AddMinutes(-1);
            DateTime processUsageTo  = DateTime.UtcNow;

            var activityToRegister = new ActivityUpdateDto()
            {
                //device info
                DeviceId = deviceId,
                DeviceName = computerName,
                DeviceTypeId = (int)DeviceType.Desktop,
                OSTypeId = (int)OSType.Windows,

                //process info
                ProcessName = processName,
                Resource = resourceName,
                ResourceDescription = resourceDescription,
                TimeFrom = processUsageFrom,
                TimeTo = processUsageTo,
                DurationInSec = (processUsageTo - processUsageFrom).TotalSeconds,
            };

            var res  = dataService.RegisterActivity(activityToRegister);

            if (res.ResponseStatus == ResponseStatus.Completed 
                && res.StatusCode == System.Net.HttpStatusCode.Created)
            {
                //Uspeshen e zapisa - trieme ot opashkata
            }
        }

        [TestMethod]
        public void Test_Login()
        {
            string apiUrl = "http://localhost:52359/Api";
            string authKey = "";
            string computerName = "DOBI_PC";
            int deviceTypeId = (int)DeviceType.Desktop;
            int oSTypeId = (int)OSType.Windows;

            string username = "tbmihailov3";
            string password = "test123";
            TimeTrackerDataService dataService = new TimeTrackerDataService(apiUrl, authKey);

            var loginModelWithDevice = new LoginModelWithDevice(){
                UserName = username,
                Password = password,
                DeviceName = computerName,
                DeviceType = deviceTypeId,
                DeviceOSType = oSTypeId
            };
            
            var response = dataService.LoginWithDevice(loginModelWithDevice);
            if (response.ResponseStatus == ResponseStatus.Completed
                && response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var userProfile = response.Data;
                Debug.WriteLine("DeviceId:"+userProfile.DeviceId.ToString());
            }
        }
    }
}
