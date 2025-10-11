using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using StudyGroups.DAL;
using StudyGroups.Filters;
using StudyGroups.Models;

namespace StudyGroups.Controllers
{
    [SessionAuthorize]
    public class StudyGroupsController : Controller
    {
        
        private StudyGroupContext db = new StudyGroupContext();

        // GET: StudyGroups
       
        public ActionResult Index(string searchString)
        {
            var studyGroups = db.StudyGroups.Include(s => s.Creator).Include(s => s.Subject);

            System.Diagnostics.Debug.WriteLine("Search String: " + searchString);


            if (!String.IsNullOrEmpty(searchString))
            {
                studyGroups = studyGroups.Where(s => s.Name.Contains(searchString)
                                                || s.Description.Contains(searchString)
                                                || s.Subject.Title.Contains(searchString));
            }
            return View(studyGroups.ToList());
        }

        // GET: StudyGroups/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StudyGroup studyGroup = db.StudyGroups.Find(id);
            if (studyGroup == null)
            {
                return HttpNotFound();
            }
            return View(studyGroup);
        }

        // GET: StudyGroups/Create
        [SessionAuthorize(Roles = "User")]
        public ActionResult Create()
        {
            ViewBag.SubjectID = new SelectList(db.Subjects, "SubjectID", "Title");
            return View();
        }

        // POST: StudyGroups/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionAuthorize(Roles = "User")]
        public ActionResult Create([Bind(Include = "StudyGroupID,Name,Description,SubjectID,CreatorUserID")] StudyGroup studyGroup)
        {
            if (ModelState.IsValid)
            {
                //the creator is the current logged in user
                studyGroup.CreatorUserID = (int)Session["UserID"];

                db.StudyGroups.Add(studyGroup);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.SubjectID = new SelectList(db.Subjects, "SubjectID", "Title", studyGroup.SubjectID);
            return View(studyGroup);
        }

        // GET: StudyGroups/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StudyGroup studyGroup = db.StudyGroups.Find(id);
            if (studyGroup == null)
            {
                return HttpNotFound();
            }

            // check if the current logged in user is the creator
            int currentUserID = (int)Session["UserID"];
            if (studyGroup.CreatorUserID != currentUserID)
            {
                TempData["Error"] = "You can only edit study groups you created.";
                return RedirectToAction("Details", new { id = id });
            }

            ViewBag.SubjectID = new SelectList(db.Subjects, "SubjectID", "Title", studyGroup.SubjectID);
            return View(studyGroup);
        }

        // POST: StudyGroups/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "StudyGroupID,Name,Description,SubjectID,CreatorUserID")] StudyGroup studyGroup)
        {
            //verify ownership
            int currentUserID = (int)Session["UserID"];
            var originalGroup = db.StudyGroups.FirstOrDefault(sg => sg.StudyGroupID == studyGroup.StudyGroupID);

            if (originalGroup == null)
            {
                return HttpNotFound();
            }

            if (originalGroup.CreatorUserID != currentUserID)
            {
                TempData["Error"] = "You can only edit study group you created.";
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                studyGroup.CreatorUserID = originalGroup.CreatorUserID;
                db.Entry(studyGroup).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.SubjectID = new SelectList(db.Subjects, "SubjectID", "Title", studyGroup.SubjectID);
            return View(studyGroup);
        }

        // GET: StudyGroups/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            StudyGroup studyGroup = db.StudyGroups.Find(id);
            if (studyGroup == null)
            {
                return HttpNotFound();
            }

            //check if the current user is the creator
            int currentUserID = (int)Session["UserID"];
            if (studyGroup.CreatorUserID != currentUserID)
            {
                TempData["Error"] = "You can only delete study groups you created.";
                return RedirectToAction("Details", new {id = id});
            }

            return View(studyGroup);
        }

        // POST: StudyGroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            StudyGroup studyGroup = db.StudyGroups.Find(id);

            if (studyGroup == null) 
            {
                return HttpNotFound();
            }

            //check if the current user is the creator
            int currentUserID = (int)Session["UserID"];
            if (studyGroup.CreatorUserID != currentUserID)
            {
                TempData["Error"] = "You can only delete study groups you created. ";
                return RedirectToAction("Index");
            }

            db.StudyGroups.Remove(studyGroup);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        // GET: StudyGroups/CreateForSubject/5
        [SessionAuthorize(Roles = "User")]
        public ActionResult CreateForSubject(int? id)
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

            // Create a new StudyGroup with pre-filled subject 
            var studyGroup = new StudyGroup
            {
                SubjectID = subject.SubjectID
            };

            // Pass the subject details to the view
            ViewBag.SubjectTitle = subject.Title;
            ViewBag.SubjectID = subject.SubjectID;
            /// flag so that we can use the same create view
            ViewBag.IsSubjectLocked = true;

            return View("Create", studyGroup);

        }

        // POST: StudyGroups/CreateForSubject
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionAuthorize(Roles = "User")]
        public ActionResult CreateForSubject([Bind(Include = "StudyGroupID,Name,Description,SubjectID")] StudyGroup studyGroup)
        {
            if (ModelState.IsValid)
            {
                // The creator is the current logged in user
                studyGroup.CreatorUserID = (int)Session["UserID"];

                db.StudyGroups.Add(studyGroup);
                db.SaveChanges();
                return RedirectToAction("Index");
            }


            // Reload subject info if validation fails
            Subject subject = db.Subjects.Find(studyGroup.SubjectID);
            if (subject != null)
            {
                ViewBag.SubjectTitle = subject.Title;
                ViewBag.SubjectID = subject.SubjectID;
                ViewBag.IsSubjectLocked = true;
            }

            return View("Create", studyGroup);
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
