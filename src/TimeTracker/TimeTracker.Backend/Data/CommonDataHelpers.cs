using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TimeTracker.Backend.Models
{
    public static class CommonDataHelpers
    {
        public static ActivityCategorization GetActivityCategorization(this TimeTrackerContext db, Guid consumerId, string processName, string resource)
        {
            var categorization = db.ActivityCategorizations.Where(p => p.ConsumerId == consumerId && p.ProcessName == processName && p.Resource == resource)
                                    .FirstOrDefault();



            return categorization;
        }

    }
}