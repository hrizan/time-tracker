using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace TimeTracker.Windows.DataStore
{
    public class TimeTrackerEmbeddedDataContext : DbContext
    {
        public DbSet<ProcessActivity> ProcessActivities { get; set; }
    }
}
