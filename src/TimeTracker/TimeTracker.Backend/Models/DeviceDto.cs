using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TimeTracker.Backend.Models
{
    public class DeviceDto
    {
        public Guid Id { get; set; }
        public int DeviceTypeId { get; set; }
        public string Name { get; set; }
        public string FriendlyName { get; set; }

        public Guid ConsumerId { get; set; }

        public string Description { get; set; }

        public int OSTypeId { get; set; }
        public string DevicePushNotificationID { get; set; }
    }
}