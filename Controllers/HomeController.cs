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
    [SessionAuthorize]
    public class HomeController : Controller
    {
        private StudyGroupContext db = new StudyGroupContext();

        public ActionResult Index()
        {
            string userRole = Session["UserRole"] as string;

            if (userRole == "Admin")
            {
                return RedirectToAction("AdminDashboard");
            }

            int currentUserID = (int)Session["UserID"];
            var user = db.Users.Find(currentUserID);

            var viewModel = new UserDashboardViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                JoinedDate = user.JoinedDate,
                Initials = $"{user.FirstName[0]}{user.LastName[0]}".ToUpper(),

                TotalSubjects = db.StudyGroups
                    .Where(sg => sg.CreatorUserID == currentUserID)
                    .Select(sg => sg.SubjectID)
                    .Union(db.StudyGroups
                        .Where(sg => sg.Members.Any(m => m.UserID == currentUserID))
                        .Select(sg => sg.SubjectID))
                    .Distinct()
                    .Count(),

                TotalSessions = db.Sessions.Count(s => s.CreatorUserID == currentUserID)
                                    + db.Sessions.Count(s => s.Attendees.Any(a => a.UserID == currentUserID) && s.CreatorUserID != currentUserID),

                TotalTimeStudied = (db.Sessions
                            .Where(s => s.CreatorUserID == currentUserID)
                            .Sum(s => (int?)s.Duration) ?? 0) +
                            (db.Sessions
                            .Where(s => s.Attendees.Any(a => a.UserID == currentUserID) && s.CreatorUserID != currentUserID)
                            .Sum(s => (int?)s.Duration) ?? 0),

                AverageRating = db.Ratings
                                .Where(r => r.UserID == currentUserID)
                                .Select(r => (double?)r.Score)
                                .DefaultIfEmpty(0)
                                .Average() ?? 0
            };
            return View(viewModel);
        }

        public ActionResult AdminDashboard()
        {
            string userRole = Session["UserRole"] as string;
            
            if (userRole != "Admin")
            {
                return RedirectToAction("Index");
            }

            var viewModel = new AdminDashboardViewModel
            {
                TotalUsers = db.Users.Count(),
                TotalStudyGroups = db.StudyGroups.Count(),
                TotalSessions = db.Sessions.Count(),
                TotalSubjects = db.Subjects.Count()
            };

            return View(viewModel);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


    }
}