using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TimeTracker.Tools;
using TimeTracker.Backend.Models;

namespace TimeTracker.Backend.Tests
{
    [TestClass]
    public class TestGeneration
    {
        [TestMethod]
        public void Test_Data_Generation()
        {
            DataGenerator dg = new DataGenerator();

            Guid consumerId = Guid.NewGuid();
            Device device = new Device()
            {
                ConsumerId = consumerId,
                Name = "TODOR_WORK_PC",
                DeviceTypeId = (int)DeviceType.Desktop,
                OSTypeId = (int)OSType.Windows,
            };




            

            //dg.GenerateActivities(consumerId, device, )
        }
    }
}
