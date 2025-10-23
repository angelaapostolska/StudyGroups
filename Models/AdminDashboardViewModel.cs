using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StudyGroups.Models
{
	public class AdminDashboardViewModel
	{
		public int TotalSubjects { get; set; }
		public int TotalUsers { get; set; }
		public int TotalStudyGroups { get; set; }
		public int TotalSessions { get; set; }
	}
}