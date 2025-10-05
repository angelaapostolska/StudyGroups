using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace StudyGroups.Models
{
    public class Session
    {
        public int SessionID { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required]
        public int Duration { get; set; } // in minutes

        // Foreign key
        public int SubjectID { get; set; }

        // Navigation properties
        [ForeignKey("SubjectID")]
        public virtual Subject Subject { get; set; }
        public virtual ICollection<Rating> Ratings { get; set; }
    }
}