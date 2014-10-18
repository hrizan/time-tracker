using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TimeTracker.RestDataClient.TimeTracker.ClientRest;
using TimeTracker.Models;
using TimeTracker.Backend.Models;
using RestSharp;

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
    }
}
