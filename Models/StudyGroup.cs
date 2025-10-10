using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace StudyGroups.Models
{
    public class StudyGroup
    {
        public int StudyGroupID { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        // Foreign key - which subject this study group is for
        [Required]
        public int SubjectID { get; set; }

        // Ownership - user who created it
        public int CreatorUserID { get; set; }

        [ForeignKey("CreatorUserID")]
        [InverseProperty("CreatedStudyGroups")]
        public virtual User Creator { get; set; }

        // Membership - users who joined
        [InverseProperty("JoinedStudyGroups")]
        public virtual ICollection<User> Members { get; set; }

        // Navigation - the subject this group studies
        [ForeignKey("SubjectID")]
        public virtual Subject Subject { get; set; }

        // Sessions for this study group
        public virtual ICollection<Session> Sessions { get; set; }
    }
}