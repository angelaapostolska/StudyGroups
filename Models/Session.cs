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

        [Required]
        [Range(1, 10, ErrorMessage = "Maximum attendees must be between 1 and 10")]
        public int MaxAttendees { get; set; } = 10;

        // Foreign key - which study group this session belongs to
        [Required]
        public int StudyGroupID { get; set; }

        // Ownership
        public int CreatorUserID { get; set; }

        [ForeignKey("CreatorUserID")]
        [InverseProperty("CreatedSessions")]
        public virtual User Creator { get; set; }

        // Attendees 
        [InverseProperty("AttendingSessions")]
        public virtual ICollection<User> Attendees { get; set; }

        // Navigation properties
        [ForeignKey("StudyGroupID")]
        public virtual StudyGroup StudyGroup { get; set; }

        public virtual ICollection<Rating> Ratings { get; set; }

        [NotMapped]
        public bool IsFinished
        {
            get
            {
                return Date.AddMinutes(Duration) < DateTime.Now;
            }
        }
    }
}