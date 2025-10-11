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
using StudyGroups.Filters;
using static System.Collections.Specialized.BitVector32;

namespace StudyGroups.Controllers
{
    [SessionAuthorize]
    public class RatingsController : Controller
    {
        private StudyGroupContext db = new StudyGroupContext();

        // GET: Ratings
        public ActionResult Index()
        {
            var ratings = db.Ratings.Include(r => r.Session).Include(r => r.User);
            return View(ratings.ToList());
        }

        // GET: Ratings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rating rating = db.Ratings.Find(id);
            if (rating == null)
            {
                return HttpNotFound();
            }
            return View(rating);
        }

        // GET: Ratings/Create
        [SessionAuthorize(Roles = "User")]
        public ActionResult Create()
        {
            ViewBag.SessionID = new SelectList(db.Sessions, "SessionID", "SessionID");
            return View();
        }

        // POST: Ratings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionAuthorize(Roles = "User")]
        public ActionResult Create([Bind(Include = "RatingID,Score,SessionID")] Rating rating)
        {
            if (ModelState.IsValid)
            {
                int currentUserID = (int)Session["UserID"];
                rating.UserID = currentUserID;

                db.Ratings.Add(rating);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.SessionID = new SelectList(db.Sessions, "SessionID", "SessionID", rating.SessionID);
            return View(rating);
        }

        // GET: Ratings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Rating rating = db.Ratings.Find(id);
            if (rating == null)
            {
                return HttpNotFound();
            }

            int currentUserID = (int)Session["UserID"];
            if (rating.UserID != currentUserID)
            {
                TempData["Error"] = "You can only edit ratings you created.";
                return RedirectToAction("Details", new { id = id });
            }

            ViewBag.SessionID = new SelectList(db.Sessions, "SessionID", "SessionID", rating.SessionID);
            return View(rating);
        }

        // POST: Ratings/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RatingID,Score,UserID,SessionID")] Rating rating)
        {
            int currentUserID = (int)Session["UserID"];
            var originalRating = db.Ratings.AsNoTracking().FirstOrDefault(r => r.RatingID == rating.RatingID);

            if (originalRating == null)
            {
                return HttpNotFound();
            }

            if (originalRating.UserID != currentUserID)
            {
                TempData["Error"] = "You can only edit ratings you created.";
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                rating.UserID = originalRating.UserID;

                db.Entry(rating).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.SessionID = new SelectList(db.Sessions, "SessionID", "SessionID", rating.SessionID);
            return View(rating);
        }

        // GET: Ratings/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Rating rating = db.Ratings.Find(id);
            if (rating == null)
            {
                return HttpNotFound();
            }

            int currentUserID = (int)Session["UserID"];
            if (rating.UserID != currentUserID)
            {
                TempData["Error"] = "You can only delete ratings you created.";
                return RedirectToAction("Details", new { id = id });
            }

            return View(rating);
        }

        // POST: Ratings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Rating rating = db.Ratings.Find(id);

            if (rating == null)
            {
                return HttpNotFound();
            }

            int currentUserID = (int)Session["UserID"];
            if (rating.UserID != currentUserID)
            {
                TempData["Error"] = "You can only delete ratings you created.";
                return RedirectToAction("Index");
            }

            db.Ratings.Remove(rating);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        // POST: Create rating from session's details
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionAuthorize(Roles = "User")]
        public ActionResult CreateFromSession(int sessionId, int score)
        {
            int currentUserID = (int)Session["UserID"];

            // verifying if the session has ended
            var session = db.Sessions
                .Include(s => s.Attendees)
                .FirstOrDefault(s => s.SessionID == sessionId);

            if (session == null)
            {
                TempData["Error"] = "Session not found.";
                return RedirectToAction("Index", "Sessions");
            }
            DateTime sessionEndTime = session.Date.AddMinutes(session.Duration);
            if (DateTime.Now <= sessionEndTime)
            {
                TempData["Error"] = "You can only rate a session after it has ended.";
                return RedirectToAction("Details", "Sessions", new { id = sessionId });
            }

            // check if the logged in user is creator or attendee
            bool isCreator = session.CreatorUserID == currentUserID;
            bool isAttendee = session.Attendees.Any(a => a.UserID == currentUserID);

            if (!isCreator && !isAttendee)
            {
                TempData["Error"] = "Only the creator and attendees can rate this session.";
                return RedirectToAction("Details", "Sessions", new { id = sessionId });
            }

            // check if the logged in user has already voted
            if (db.Ratings.Any(r => r.SessionID == sessionId && r.UserID == currentUserID))
            {
                TempData["Error"] = "You have already rated this session.";
                return RedirectToAction("Details", "Sessions", new { id = sessionId });
            }

            // create the rating
            var rating = new Rating
            {
                Score = score,
                UserID = currentUserID,
                SessionID = sessionId
            };

            db.Ratings.Add(rating);
            db.SaveChanges();

            TempData["Success"] = "Rating submitted successfully!";
            return RedirectToAction("Details", "Sessions", new { id = sessionId });
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