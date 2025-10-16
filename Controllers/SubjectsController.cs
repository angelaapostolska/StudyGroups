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
    [SessionAuthorize]
    public class SubjectsController : Controller
    {

        private StudyGroupContext db = new StudyGroupContext();

        // GET: Subjects
        public ActionResult Index(string searchString)
        {
            var subjects = db.Subjects.AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
                subjects = subjects.Where(s => s.Title.Contains(searchString) ||
                                              s.Description.Contains(searchString));
            }

            return View(subjects.ToList());
        }

        // GET: Subjects/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Subject subject = db.Subjects.Find(id);
            if (subject == null)
            {
                return HttpNotFound();
            }
            return View(subject);
        }

        // GET: Subjects/Create
        [SessionAuthorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Subjects/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionAuthorize(Roles = "Admin")]
        public ActionResult Create([Bind(Include = "SubjectID,Title,Description")] Subject subject)
        {
            if (ModelState.IsValid)
            {
                db.Subjects.Add(subject);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Subject created successfully!";
                return RedirectToAction("Index");
            }

            return View(subject);
        }

        // GET: Subjects/Edit/5
        [SessionAuthorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Subject subject = db.Subjects.Find(id);
            if (subject == null)
            {
                return HttpNotFound();
            }
            return View(subject);
        }

        // POST: Subjects/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionAuthorize(Roles = "Admin")]
        public ActionResult Edit([Bind(Include = "SubjectID,Title,Description")] Subject subject)
        {
            if (ModelState.IsValid)
            {
                db.Entry(subject).State = EntityState.Modified;
                TempData["SuccessMessage"] = "Subject updated successfully!";
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(subject);
        }


        // GET: Subjects/Delete/5
        [SessionAuthorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Subject subject = db.Subjects.Find(id);
            if (subject == null)
            {
                return HttpNotFound();
            }
            return View(subject);
        }

        // POST: Subjects/Delete/5
        [SessionAuthorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Subject subject = db.Subjects.Find(id);
            db.Subjects.Remove(subject);
            db.SaveChanges();
            TempData["SuccessMessage"] = "Subject deleted successfully!";

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
