using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using TimeTracker.Backend.Models;

namespace TimeTracker.Backend.Models
{
    [Table("Consumers")]
    public class Consumer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Occupation { get; set; }

        public DateTime? Created { get; set; }
        public string Username { get; set; }
        
        public virtual ICollection<Device> Devices { get; set; }
        public virtual ICollection<Goal> Goals { get; set; }
        public virtual ICollection<Activity> Activities { get; set; }
        public virtual ICollection<Category> Categories { get; set; }
        public virtual ICollection<ActivityCategorization> ActivityCategorizations { get; set; }

        //1-Male, 2-Female
        public int Sex { get; set; }
    }
}