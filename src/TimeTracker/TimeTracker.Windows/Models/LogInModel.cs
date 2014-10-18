using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeTracker.Windows.Models
{
     public class LogInModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string DeviceName { get; set; }
        public int DeviceOSType { get; set; }
        public string DeviceJsonInfo { get; set; }
    }
}
