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

namespace StudyGroups.Controllers
{
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
        [SessionAuthorize]
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
        [SessionAuthorize]
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
        [SessionAuthorize]
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
        [SessionAuthorize]
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