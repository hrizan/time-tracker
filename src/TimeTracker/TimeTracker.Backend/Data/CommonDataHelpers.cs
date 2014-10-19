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
            processName = processName.ToLower();
            resource = resource.ToLower();
            var categorization = db.ActivityCategorizations
                                    .Where(p => (p.ConsumerId == consumerId || p.ConsumerId == null) 
                                            && ((p.ProcessName == processName && p.Resource == resource)
                                            ||(p.ProcessName == null && p.Resource == resource)
                                            ||(p.ProcessName == processName && p.Resource == null)
                                            )
                                            )
                                    .FirstOrDefault();

            return categorization;
        }

    }
}