using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TimeTracker.Backend.Models
{
    public class UserProfileWithDeviceDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string AuthToken { get; set; }
        public Guid? ConsumerId { get; set; }
        public Guid? DeviceId { get; set; }
    }
}