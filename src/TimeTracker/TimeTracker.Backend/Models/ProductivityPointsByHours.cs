using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TimeTracker.Backend.Models
{
    public class ProductivityPointsByHours
    {
        public List<int> Productive { get; set; }
        public List<int> Distractive { get; set; }
    }
}