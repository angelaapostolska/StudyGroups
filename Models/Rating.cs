using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace StudyGroups.Models
{
    public class Rating
    {
        public int RatingID { get; set; }

        [Required]
        [Range(1, 5)]
        public int Score { get; set; }

        // Foreign keys
        public int UserID { get; set; }
        public int SessionID { get; set; }

        // Navigation properties
        [ForeignKey("UserID")]
        public virtual User User { get; set; }

        [ForeignKey("SessionID")]
        public virtual Session Session { get; set; }

    }
}
   