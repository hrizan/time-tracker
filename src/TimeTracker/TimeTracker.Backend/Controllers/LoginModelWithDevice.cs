using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeTracker.Backend.Controllers
{
    public class LoginModelWithDevice
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string DeviceName { get; set; }
        public int DeviceOSType { get; set; }
        public string DeviceJsonInfo { get; set; }
    }
}
