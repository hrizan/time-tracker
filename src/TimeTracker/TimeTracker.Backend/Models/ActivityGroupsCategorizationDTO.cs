using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TimeTracker.Backend.Models
{
    public class ActivityGroupsCategorizationDTO
    {
        public string ProcessName { get; set; }
        public string Resource { get; set; }

        public ActivityCategorization ActivityCategorization{ get; set; }

        public Guid? CategoryId { get; set; }
        public virtual Category Category { get; set; }

        public int ProductivityScore { get; set; }


    }
}