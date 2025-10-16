using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StudyGroups.Models
{
	public class UserDashboardViewModel
	{
		// basic user info
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public string Initials { get; set; }
		public DateTime JoinedDate { get; set; }

        // statistics calculated
		public int TotalSubjects { get; set; }
		public int TotalSessions { get; set; }
		public int TotalTimeStudied { get; set; } 
		public double AverageRating { get; set; }
      
		// formated for the view
		public string FullName => $"{FirstName} {LastName}";
		public string FormattedJoinedDate => JoinedDate.ToString("MMMM, yyyy");

		public string FormattedTimeStudied
		{
			get
			{
				if (TotalTimeStudied < 60)
                    return $"{TotalTimeStudied} mins";
                else
                {
                    int hours = TotalTimeStudied / 60;
                    int minutes = TotalTimeStudied % 60;
                    return $"{hours} hrs {minutes} mins";
                }
            }
		}

		public string FormattedRating => AverageRating > 0 ? AverageRating.ToString("0.0") : "N/A";
    }
}