using TimeTracker.Backend.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace TimeTracker.Backend.Models
{
    [Table("Devices")]
    public class Device
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public int DeviceTypeId { get; set; }
        public string Name { get; set; }
        public string FriendlyName { get; set; }
        
        public Guid ConsumerId { get; set; }
        public virtual Consumer Consumer { get; set; }
        
        public string Description { get; set; }

        public int OSTypeId { get; set; }
        public string DevicePushNotificationID { get; set; }
    }
}
