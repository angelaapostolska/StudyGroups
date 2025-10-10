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

        [StringLength(500)]
        public string Description {  get; set; }

     

        // Navigation properties - has multiple study groups
        public virtual ICollection<StudyGroup> StudyGroups { get; set; }
    }
}