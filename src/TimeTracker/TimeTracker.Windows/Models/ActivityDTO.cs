using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeTracker.Windows.Models
{
    public class ActivityDTO
    {
        public DateTime TimeFrom{ get; set; }
        public DateTime TimeTo { get; set; }
        public int Duration { get; set; }



    }
}
