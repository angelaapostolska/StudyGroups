using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StudyGroups.DAL;
using StudyGroups.Filters;
using StudyGroups.Models;

namespace StudyGroups.Controllers
{
    [SessionAuthorize(Roles = "Admin")]
    public class UsersController : Controller
    {

        private StudyGroupContext db = new StudyGroupContext();

        // GET: Users
        public ActionResult Index()
        {

            var usersData = db.Users
                .Select(u => new
                {
                    UserID = u.UserID,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    JoinedDate = u.JoinedDate,
                    CreatedGroupsCount = db.StudyGroups.Count(sg => sg.CreatorUserID == u.UserID),
                    JoinedGroupsCount = db.StudyGroups.Count(sg => sg.Members.Any(m => m.UserID == u.UserID)),
                    CreatedSessionsCount = db.Sessions.Count(s => s.CreatorUserID == u.UserID),
                    AttendingSessionsCount = db.Sessions.Count(s => s.Attendees.Any(a => a.UserID == u.UserID))
                })
                .OrderByDescending(u => u.JoinedDate)
                .ToList();

            var users = usersData.Select(u => new UserListViewModel
            {
                UserID = u.UserID,
                FullName = u.FirstName + " " + u.LastName,
                Email = u.Email,
                JoinedDate = u.JoinedDate,
                FormattedJoinedDate = u.JoinedDate.ToString("MMM dd, yyyy"),
                StudyGroupsCount = u.CreatedGroupsCount + u.JoinedGroupsCount,
                SessionsCount = u.CreatedSessionsCount + u.AttendingSessionsCount
            }).ToList();

            return View(users);
        }
    }
}