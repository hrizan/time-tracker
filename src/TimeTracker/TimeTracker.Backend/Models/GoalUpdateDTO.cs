using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TimeTracker.Backend.Models
{
    public class GoalUpdateDTO
    {
        public Guid Id { get; set; }
        public int GoalTargetId { get; set; }

        public Guid? CategoryId { get; set; }

        public int Value { get; set; }
    }
}