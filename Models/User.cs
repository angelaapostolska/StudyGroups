using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace StudyGroups.Models
{
    public class User
    {
        public int UserID { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name ="First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[A-Za-z\d]{8,}$",
        ErrorMessage = "Password must be at least 8 characters with uppercase, lowercase, and number.")]
        public string Password { get; set; }

    
        public string Role { get; set; }

        // Study Groups
        // Study Groups created by the user (ownership)
        [InverseProperty("Creator")]
        public virtual ICollection<StudyGroup> CreatedStudyGroups { get; set; }

        // Study groups that the user is member of (memberships)
        [InverseProperty("Members")]
        public virtual ICollection<StudyGroup> JoinedStudyGroups { get; set; }

        // Sessions
        // Sessions this user created (ownership)
        [InverseProperty("Creator")]
        public virtual ICollection<Session> CreatedSessions { get; set; }

        // Sessions that the user is attending (memberships)
        [InverseProperty("Attendees")]
        public virtual ICollection<Session> AttendingSessions { get; set; }

    }
}