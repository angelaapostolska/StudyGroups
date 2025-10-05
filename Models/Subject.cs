using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static System.Collections.Specialized.BitVector32;

namespace StudyGroups.Models
{
    public class Subject
    {
        public int SubjectID { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        // Foreign key
        public int StudyGroupID { get; set; }

        // Navigation properties
        [ForeignKey("StudyGroupID")]
        public virtual StudyGroup StudyGroup { get; set; }
        public virtual ICollection<Session> Sessions { get; set; }
    }
}