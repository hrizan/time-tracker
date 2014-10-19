using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TimeTracker.Backend.Models
{
    public class ProductivityByCategoryByHoursDTO
    {
        public string Label { get; set; }
        public List<int> Values{ get; set; }
    }
}