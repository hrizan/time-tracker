using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TimeTracker.Backend.Models
{
    public class CategoryUpdateDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}