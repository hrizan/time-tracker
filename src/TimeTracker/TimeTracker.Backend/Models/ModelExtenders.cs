using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TimeTracker.Backend.Models
{
    public static class ModelExtenders
    {
        public static IQueryable<Activity> ForConsumer(this IQueryable<Activity> activities, Guid consumerId)
        {
            return activities.Where(a => a.ConsumerId == consumerId);
        }

        public static IQueryable<Category> ForConsumer(this IQueryable<Category> categories, Guid consumerId)
        {
            return categories.Where(a => a.ConsumerId == null || a.ConsumerId == consumerId);
        }

        public static IQueryable<Goal> ForConsumer(this IQueryable<Goal> goals, Guid consumerId)
        {
            return goals.Where(a => a.ConsumerId == consumerId);
        }

        public static IQueryable<Device> ForConsumer(this IQueryable<Device> devices, Guid consumerId)
        {
            return devices.Where(a => a.ConsumerId == consumerId);
        }

        public static IQueryable<ActivityCategorization> ForConsumer(this IQueryable<ActivityCategorization> activityCategorizations, Guid consumerId)
        {
            return activityCategorizations.Where(a => a.ConsumerId == consumerId || a.ConsumerId == null);
        }
    }
}