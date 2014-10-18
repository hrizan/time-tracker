using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Windows.Forms;
using TimeTracker.Windows.DataStore;

namespace TimeTracker.Windows
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Database.DefaultConnectionFactory = new
                        SqlCeConnectionFactory("System.Data.SqlServerCe.4.0");
            Database.SetInitializer(
                         new DropCreateDatabaseIfModelChanges<TimeTrackerEmbeddedDataContext>()
                  );

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new TymestTrayContext());
        }
    }
}
