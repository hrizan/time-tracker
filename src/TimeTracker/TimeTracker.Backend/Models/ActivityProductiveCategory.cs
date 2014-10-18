using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TimeTracker.Backend.Models
{
    public class ActivityProductiveCategory
    {
        public Guid Id { get; set; }
        public string ProcessName { get; set; }
        public string Resource { get; set; }
        
    }
}