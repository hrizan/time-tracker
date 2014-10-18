using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace TimeTracker.Backend.Models
{
    [Table("ActivityCategorizations")]
    public class ActivityCategorization
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid? ConsumerId { get; set; }
        public virtual Consumer Consumer { get; set; }
        
        public string ProcessName { get; set; }
        public string Resource { get; set; }

        public Guid? CategoryId { get; set; }
        public virtual Category Category { get; set; }

        public int ProductivityScore { get; set; }
    }
}
