using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace TimeTracker.Windows.DataStore
{
    [Table("Activities")]
    public class ProcessActivity
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Time UTC(0) DateTime.Now.UtcNow
        /// </summary>
        public DateTime TimeFrom { get; set; }
        public DateTime TimeTo { get; set; }
        public double DurationInSec { get; set; }

        public Guid DeviceId { get; set; }

        //public int DeviceTypeId { get; set; }
        public string DeviceName { get; set; }

        /// <summary>
        /// Example: Chrome.exe
        /// </summary>
        public string ProcessName { get; set; }

        /// <summary>
        /// Example : Facebook.com
        /// </summary>
        public string Resource { get; set; }

        /// <summary>
        /// Title for web pages for example
        /// </summary>
        public string ResourceDescription { get; set; }

        public bool IsSynced { get; set; }
    }
}
