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
        public ActionResult Index()
        {
            var sessions = db.Sessions.Include(s => s.Creator).Include(s => s.StudyGroup);
            return View(sessions.ToList());
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