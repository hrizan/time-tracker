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
        public int Duration { get; set; }

        public Guid DeviceId { get; set; }
        public virtual Device Device { get; set; }

        public int DeviceType { get; set; }

        public int ProductivityScore { get; set; }
        
        public Guid CategoryId { get; set; }
        public virtual Category Category { get; set; }
    }
}
