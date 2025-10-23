using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StudyGroups.Models
{
	public class UserListViewModel
	{
		public int UserID { get; set; }
		public string FullName { get; set; }
        public string Email { get; set; }
		public string FormattedJoinedDate { get; set; }
		public DateTime JoinedDate { get; set; }
		public int StudyGroupsCount { get; set; }
		public int SessionsCount { get; set; }	

    }
}