using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeTracker.Windows.DataStore
{
    public class TimeTrackerEmbeddedDataService
    {
        TimeTrackerEmbeddedDataContext _db = new TimeTrackerEmbeddedDataContext();
        
        public TimeTrackerEmbeddedDataService()
        {

        }

        public IQueryable<ProcessActivity> GetProcessActivities(int limitNumber)
        {
            return _db.ProcessActivities.Take(limitNumber);
        }

        public void DeleteProcessActivity(Guid id)
        {
            var activity = _db.ProcessActivities.Where(a => a.Id == id).FirstOrDefault();
            _db.ProcessActivities.Remove(activity);

            _db.SaveChanges();
        }
        
    }
}
