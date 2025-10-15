using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using StudyGroups.DAL;
using StudyGroups.Models;
using StudyGroups.Filters; // ADD THIS LINE

namespace StudyGroups.Controllers
{
    [SessionAuthorize]
    public class SessionsController : Controller
    {
        private StudyGroupContext db = new StudyGroupContext();

        // GET: Sessions
        public ActionResult Index(string searchString, int? studyGroupId, int? page)
        {
            int? currentUserID = Session["UserID"] as int?;

            // filtering by study groups where the user is a member or the creator  
            
            if (currentUserID.HasValue)
            {
                var userStudyGroups = db.StudyGroups
                .Include(sg => sg.Members)
                .Where(sg => sg.CreatorUserID == currentUserID.Value ||
                            sg.Members.Any(m => m.UserID == currentUserID.Value))
                .OrderBy(sg => sg.Name)
                .ToList();

                ViewBag.StudyGroups = new SelectList(userStudyGroups, "StudyGroupID", "Name", studyGroupId);
            }
            else
            {
                ViewBag.StudyGroups = new SelectList(Enumerable.Empty<SelectListItem>());
            }

            DateTime now = DateTime.Now;

            var sessions = db.Sessions
                .Include(s => s.Creator)
                .Include(s => s.StudyGroup)
                .Include(s => s.StudyGroup.Subject)
                .Include(s => s.StudyGroup.Members)
                .Include(s => s.Attendees)
                .AsEnumerable(); 

       
            // user is the member of the study group where the session belongs, or user is the creator
            if (currentUserID.HasValue)
            {
                sessions = sessions.Where(s =>
                    s.CreatorUserID == currentUserID.Value ||
                    (s.StudyGroup.Members != null && s.StudyGroup.Members.Any(m => m.UserID == currentUserID.Value))
                );
            }

            // search filtering
            if (!String.IsNullOrEmpty(searchString))
            {
                sessions = sessions.Where(s =>
                    s.StudyGroup.Name.Contains(searchString) ||
                    s.StudyGroup.Subject.Title.Contains(searchString) ||
                    s.Creator.FirstName.Contains(searchString) ||
                    s.Creator.LastName.Contains(searchString)
                );
            }

            // study group filter
            if (studyGroupId.HasValue && studyGroupId.Value > 0)
            {
                sessions = sessions.Where(s => s.StudyGroupID == studyGroupId.Value);
            }

            // order by date and filter only upcoming
            sessions = sessions.OrderBy(s => s.Date);
            var allSessions = sessions.ToList();
            var upcomingSessions = allSessions.Where(s => s.Date.AddMinutes(s.Duration) > now).ToList();

            // pagination
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            var totalItems = upcomingSessions.Count();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var pagedSessions = upcomingSessions
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();


            ViewBag.SearchString = searchString;
            ViewBag.StudyGroupId = studyGroupId;
            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;


            return View(pagedSessions);
        }

        // GET: Sessions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Session session = db.Sessions
                .Include(s => s.Creator)
                .Include(s => s.StudyGroup)
                .Include(s => s.Attendees)
                .Include(s => s.Ratings)
                .Include(s => s.Ratings.Select(r => r.User))
                .FirstOrDefault(s => s.SessionID == id);

            if (session == null)
            {
                return HttpNotFound();
            }

            // calculate whether the session has ended yet
            DateTime sessionEndTime = session.Date.AddMinutes(session.Duration);
            ViewBag.HasSessionEnded = DateTime.Now > sessionEndTime;

            // check if the logged in user can rate
            if (System.Web.HttpContext.Current.Session["UserID"] != null)
            {
                int currentUserID = (int)System.Web.HttpContext.Current.Session["UserID"];
                bool isCreator = session.CreatorUserID == currentUserID;

                bool isAttendee = session.Attendees != null && session.Attendees.Any(a => a.UserID == currentUserID);
                bool hasAlreadyRated = session.Ratings != null && session.Ratings.Any(r => r.UserID == currentUserID);

                ViewBag.CanRate = (isCreator || isAttendee) && !hasAlreadyRated;
                ViewBag.HasAlreadyRated = hasAlreadyRated;
            }
            else
            {
                ViewBag.CanRate = false;
                ViewBag.HasAlreadyRated = false;
            }

            return View(session);
        }

        // GET: Sessions/Create
        [SessionAuthorize(Roles = "User")]
        public ActionResult Create()
        {
            // Study groups created by the user only - name conflict with session
            int currentUserID = (int)System.Web.HttpContext.Current.Session["UserID"];
            ViewBag.StudyGroupID = new SelectList(
                db.StudyGroups.Where(sg => sg.CreatorUserID == currentUserID),
                "StudyGroupID", 
                "Name"
            );
            return View();
        }

        // POST: Sessions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionAuthorize(Roles = "User")]
        public ActionResult Create([Bind(Include = "SessionID,Date,Duration,StudyGroupID")] Session session)
        {
            if (ModelState.IsValid)
            {
                session.CreatorUserID = (int)System.Web.HttpContext.Current.Session["UserID"];

                db.Sessions.Add(session);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            int currentUserID = (int)System.Web.HttpContext.Current.Session["UserID"];
            ViewBag.StudyGroupID = new SelectList(
                db.StudyGroups.Where(sg => sg.CreatorUserID == currentUserID),
                "StudyGroupID",
                "Name",
                session.StudyGroupID
            );

            return View(session);
        }

        // GET: Sessions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Session session = db.Sessions.Find(id);
            if (session == null)
            {
                return HttpNotFound();
            }

            //  only the creator can edit
            int currentUserID = (int)System.Web.HttpContext.Current.Session["UserID"];
            if (session.CreatorUserID != currentUserID)
            {
                TempData["Error"] = "You can only edit sessions you created.";
                return RedirectToAction("Details", new { id = id });
            }

            ViewBag.StudyGroupID = new SelectList(
                db.StudyGroups.Where(sg => sg.CreatorUserID == currentUserID),
                "StudyGroupID",
                "Name",
                session.StudyGroupID
            );

            return View(session);
        }

        // POST: Sessions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SessionID,Date,Duration,StudyGroupID,CreatorUserID")] Session session)
        {
            int currentUserID = (int)System.Web.HttpContext.Current.Session["UserID"];
            var originalSession = db.Sessions.AsNoTracking().FirstOrDefault(s => s.SessionID == session.SessionID);

            if (originalSession == null)
            {
                return HttpNotFound();
            }

            if (originalSession.CreatorUserID != currentUserID)
            {
                TempData["Error"] = "You can only edit sessions you created.";
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                session.CreatorUserID = originalSession.CreatorUserID;

                db.Entry(session).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.StudyGroupID = new SelectList(
                db.StudyGroups.Where(sg => sg.CreatorUserID == currentUserID),
                "StudyGroupID",
                "Name",
                session.StudyGroupID
            );

            return View(session);
        }

        // GET: Sessions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Session session = db.Sessions.Find(id);
            if (session == null)
            {
                return HttpNotFound();
            }

            int currentUserID = (int)System.Web.HttpContext.Current.Session["UserID"];
            if (session.CreatorUserID != currentUserID)
            {
                TempData["Error"] = "You can only delete sessions you created.";
                return RedirectToAction("Details", new { id = id });
            }

            return View(session);
        }

        // POST: Sessions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Session session = db.Sessions.Find(id);

            if (session == null)
            {
                return HttpNotFound();
            }

     
            int currentUserID = (int)System.Web.HttpContext.Current.Session["UserID"];
            if (session.CreatorUserID != currentUserID)
            {
                TempData["Error"] = "You can only delete sessions you created.";
                return RedirectToAction("Index");
            }

            db.Sessions.Remove(session);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Sessions/CreateForStudyGroup/5
        [SessionAuthorize(Roles = "User")]
        public ActionResult CreateForStudyGroup(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            StudyGroup studyGroup = db.StudyGroups
                .Include(sg => sg.Subject)
                .Include(sg => sg.Members)
                .FirstOrDefault(sg => sg.StudyGroupID == id);

            if (studyGroup == null)
            {
                return HttpNotFound();
            }

            // check if the current user is creator or member
            int currentUserID = (int)Session["UserID"];
            bool isCreator = studyGroup.CreatorUserID == currentUserID;
            bool isMember = studyGroup.Members != null && studyGroup.Members.Any(m => m.UserID == currentUserID);

            if (!isCreator && !isMember)
            {
                TempData["Error"] = "You must be the creator or a member of this study group to create sessions.";
                return RedirectToAction("Details", "StudyGroups", new { id = id });
            }

            // create a new session with pre-filled study group value
            var session = new Session
            {
                StudyGroupID = studyGroup.StudyGroupID
            };

            // pass the study group details to the view
            ViewBag.StudyGroupName = studyGroup.Name;
            ViewBag.SubjectTitle = studyGroup.Subject.Title;
            ViewBag.StudyGroupID = studyGroup.StudyGroupID;
            ViewBag.IsStudyGroupLocked = true;

            return View("Create", session);
        }

        // POST: Sessions/CreateForStudyGroup
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionAuthorize(Roles = "User")]
        public ActionResult CreateForStudyGroup([Bind(Include = "SessionID,Date,Duration,StudyGroupID")] Session session)
        {
            StudyGroup studyGroup = db.StudyGroups
               .Include(sg => sg.Subject)
               .Include(sg => sg.Members)
               .FirstOrDefault(sg => sg.StudyGroupID == session.StudyGroupID);

            if (studyGroup == null)
            {
                return HttpNotFound();
            }

            int currentUserID = (int)Session["UserID"];
            bool isCreator = studyGroup.CreatorUserID == currentUserID;
            bool isMember = studyGroup.Members != null && studyGroup.Members.Any(m => m.UserID == currentUserID);

            if (!isCreator && !isMember)
            {
                TempData["Error"] = "You must be the creator or a member of this study group to create sessions.";
                return RedirectToAction("Details", "StudyGroups", new { id = session.StudyGroupID });
            }

            // validation check for time overlapping between sessions
            DateTime sessionEndTime = session.Date.AddMinutes(session.Duration);

            var existingSessions = db.Sessions
                .Where(s => s.StudyGroupID == session.StudyGroupID)
                .ToList();

            bool isOverlapping = existingSessions.Any(s =>
            {
                DateTime existingEndTime = s.Date.AddMinutes(s.Duration);
                return (session.Date >= s.Date && session.Date < existingEndTime) ||
                       (sessionEndTime > s.Date && sessionEndTime <= existingEndTime) ||
                       (session.Date <= s.Date && sessionEndTime >= existingEndTime);
            });


            if (isOverlapping)
            {
                ModelState.AddModelError("Date", "A session already exists at this time for this study group. Sessions cannot overlap.");
            }

            if (ModelState.IsValid)
            {
                // The creator is the current logged in user
                session.CreatorUserID = (int)Session["UserID"];

                db.Sessions.Add(session);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            // Reload study group info if validation fails
            ViewBag.StudyGroupName = studyGroup.Name;
            ViewBag.SubjectTitle = studyGroup.Subject.Title;
            ViewBag.StudyGroupID = studyGroup.StudyGroupID;
            ViewBag.IsStudyGroupLocked = true;


            return View("Create", session);

        }

        // POST: Sessions/Join/5
        [HttpPost]
        public ActionResult Join(int id)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int currentUserID = (int)Session["UserID"];
            var session = db.Sessions
                .Include(s => s.Attendees)
                .FirstOrDefault(s => s.SessionID == id);

            if (session == null)
            {
                return HttpNotFound();
            }

            // Check if user is the creator
            if (session.CreatorUserID == currentUserID)
            {
                TempData["Error"] = "You cannot join your own session.";
                return RedirectToAction("Details", new { id });
            }

            // Check if already attending
            if (session.Attendees.Any(a => a.UserID == currentUserID))
            {
                TempData["Error"] = "You are already attending this session.";
                return RedirectToAction("Details", new { id });
            }

            // Check if session is full
            if (session.Attendees.Count >= session.MaxAttendees)
            {
                TempData["Error"] = "This session is full.";
                return RedirectToAction("Details", new { id });
            }

            var user = db.Users.Find(currentUserID);
            session.Attendees.Add(user);
            db.SaveChanges();

            TempData["Success"] = "Successfully joined the session!";
            return RedirectToAction("Details", new { id });
        }

        // POST: Sessions/Leave/5
        [HttpPost]
        public ActionResult Leave(int id)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int currentUserID = (int)Session["UserID"];
            var session = db.Sessions
                .Include(s => s.Attendees)
                .FirstOrDefault(s => s.SessionID == id);

            if (session == null)
            {
                return HttpNotFound();
            }

            var user = session.Attendees.FirstOrDefault(a => a.UserID == currentUserID);
            if (user != null)
            {
                session.Attendees.Remove(user);
                db.SaveChanges();
                TempData["Success"] = "Successfully left the session.";
            }

            return RedirectToAction("Index");
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