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

namespace StudyGroups.Controllers
{
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
            Session session = db.Sessions.Find(id);
            if (session == null)
            {
                return HttpNotFound();
            }
            return View(session);
        }

        // GET: Sessions/Create
        [Authorize(Roles = "User")]
        public ActionResult Create()
        {
            //study groups created by the user only
            int currentUserID = (int)Session["UserID"];
            ViewBag.StudyGroupID = new SelectList(
                db.StudyGroups.Where(sg => sg.CreatorUserID == currentUserID),
                "StudyGroupUD",
                "Name"
                );
            return View();
        }

        // POST: Sessions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "User")]
        public ActionResult Create([Bind(Include = "SessionID,Date,Duration,StudyGroupID,CreatorUserID")] Session session)
        {
            if (ModelState.IsValid)
            {
                session.CreatorUserID = (int)Session["UserID"];

                db.Sessions.Add(session);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            int currentUserID = (int)Session["UserID"];
            ViewBag.StudyGroupID = new SelectList(
                db.StudyGroups.Where(sg => sg.CreatorUserID == currentUserID),
                "StudyGroupID",
                "Name",
                session.StudyGroupID
             );

            return View(session);
        }


        // GET: Sessions/Edit/5
        [Authorize]
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

            // check if the current logged in user is the creator
            int currentUserID = (int)Session["UserID"];
            if (session.CreatorUserID != currentUserID)
            {
                TempData["Error"] = "You can only edit sessions you created. ";
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
        [Authorize]
        public ActionResult Edit([Bind(Include = "SessionID,Date,Duration,StudyGroupID,CreatorUserID")] Session session)
        {
            int currentUserId = (int)Session["UserID"];
            var originalSession = db.Sessions.FirstOrDefault(s => s.SessionID == session.SessionID);

            if (originalSession == null)
            {
                return HttpNotFound();
            }

            if (originalSession.CreatorUserID != currentUserId)
            {
                TempData["Error"] = "You can only edit sessions you created.";
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                db.Entry(session).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.StudyGroupID = new SelectList(
                db.StudyGroups.Where(sg => sg.CreatorUserID == currentUserId),
                "StudyGroupID",
                "Name",
                session.StudyGroupID
            );

            return View(session);
        }

        // GET: Sessions/Delete/5
        [Authorize]
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

            int currentUserId = (int)Session["UserID"];
            if (session.CreatorUserID != currentUserId)
            {
                TempData["Error"] = "You can only delete sessions you created.";
                return RedirectToAction("Details", new { id = id });
            }

            return View(session);
        }

        // POST: Sessions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult DeleteConfirmed(int id)
        {
            Session session = db.Sessions.Find(id);

            if (session == null)
            {
                return HttpNotFound();
            }

            int currentUserId = (int)Session["UserID"];
            if (session.CreatorUserID != currentUserId)
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
