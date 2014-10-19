using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeTracker.Backend.Data
{
    public class Spending
    {
        public string ProcessName { get; set; }
        public string ResourceName { get; set; }
        public double ToGenerateInMinutes { get; set; }
        public double CurrentInMinutes { get; set; }
    }
}
