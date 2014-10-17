using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace TimeTracker.Backend.Models
{
        [Table("Goals")]
    public class Goal
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid ConsumerId { get; set; }
        public virtual Consumer Consumer { get; set; }

        public int GoalTargetId { get; set; }
        
        public Guid CategoryId { get; set; }
        public virtual Category Category{get;set;}
    }
}
