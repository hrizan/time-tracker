using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace TimeTracker.Backend.Models
{
    [Table("Activies")]
    public class Activity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid ConsumerId { get; set; }
        public virtual Consumer Consumer { get; set; }

        public DateTime TimeFrom { get; set; }
        public DateTime TimeTo { get; set; }
        public double DurationInSec { get; set; }

        /// <summary>
        /// Device Id
        /// </summary>
        public Guid DeviceId { get; set; }
        public virtual Device Device { get; set; }

        public int DeviceTypeId { get; set; }
        public string DeviceName { get; set; }
        public int OSTypeId { get; set; }

        /// <summary>
        /// Category of the activity: Example: Software Dev, Reading, Social Networks and so on
        /// </summary>
        public Guid? CategoryId { get; set; }
        public virtual Category Category { get; set; }

        /// <summary>
        /// Coeficcient?
        /// </summary>
        public int ProductiveMultiplier { get; set; } //Denormalized - present in ActivityProductCategory
        
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

        /// <summary>
        /// +2 Very productive, +1 Productive, 0 neutral, -1 Distractive, -2 Very Distractive
        /// </summary>
        public int ProductivityScore { get; set; }
    }
}
