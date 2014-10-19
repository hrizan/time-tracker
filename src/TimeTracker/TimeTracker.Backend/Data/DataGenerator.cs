using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using TimeTracker.Backend.Models;
using TimeTracker.Backend.Data;

namespace TimeTracker.Tools
{
    public class DataGenerator
    {

        public void GenerateUserActivities(Guid consumerId, Guid deviceId, int forLastDaysNumber)
        {
            TimeTrackerContext db = new TimeTrackerContext();
            var categories = db.Categories.ForConsumer(consumerId);
            var devices = db.Devices.ForConsumer(consumerId).FirstOrDefault(c=>c.Id==deviceId);

            var categorizations = db.ActivityCategorizations.ForConsumer(consumerId);

            foreach (var category in categories)
            {
                Debug.WriteLine(string.Format("{0} - {1}", category.Name, category.Id));
            }
        }

        

        public List<Activity> GenerateActivities(Guid consumerId, Device device, List<ActivityCategorization> categorizations, List<Spending> categorizationsSpending, DateTime date)
        {
            int minSingleProcessUsageinSec = 4*60;
            int maxSingleProcessUsageinSec = 15*60;

            List<Activity> activities = new List<Activity>();

            int randomNumber = categorizationsSpending.Count;

            double totalTimeInMinutes = 24*60;

            DateTime maxTime = date.Date.AddDays(1);
            DateTime currentTime = date.Date;
            Random random = new Random();
            while (currentTime < maxTime)
            {
                int seconds = random.Next(minSingleProcessUsageinSec, maxSingleProcessUsageinSec);

                //vzemame praznite
                var notFilledCategorizations = categorizationsSpending.Where(c=>c.ToGenerateInMinutes>c.CurrentInMinutes).ToList();    
                int catgorizationIndex = random.Next(0, notFilledCategorizations.Count-1);

                //vzemame
                var catSpending = notFilledCategorizations[catgorizationIndex];
                string processName = catSpending.ProcessName;
                string resource = catSpending.ResourceName;

                DateTime beginTime = currentTime;
                DateTime endDateTime = currentTime.AddSeconds(seconds);
                int durationInSec = seconds;

                var activity = new Activity()
                {
                    ConsumerId = consumerId,
                    ProcessName = processName,
                    Resource = resource,
                    DeviceId = device.Id,
                    DeviceName = device.Name,
                    DeviceTypeId = device.DeviceTypeId,
                    OSTypeId = device.OSTypeId,
                    TimeFrom = beginTime,
                    TimeTo = endDateTime,
                    DurationInSec = seconds
                };

                var categorization = categorizations.Where(p => (p.ConsumerId == consumerId || p.ConsumerId == null)
                                            && ((p.ProcessName == processName && p.Resource == resource)
                                            || (p.ProcessName == null && p.Resource == resource)
                                            || (p.ProcessName == processName && p.Resource == null)
                                            )
                                            )
                                    .FirstOrDefault();

                if (categorization != null)
                {
                    activity.ProductivityScore = categorization.ProductivityScore;
                    activity.CategoryId = categorization.CategoryId;
                }
                else
                {
                    activity.ProductivityScore = (int)ProductivityTypeEnum.Neutral;
                    activity.CategoryId = null;
                }

                activities.Add(activity);
                currentTime = endDateTime;
            }

            return activities;
        }

     
    }
}
