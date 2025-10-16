using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StudyGroups.DAL;
using StudyGroups.Models;

namespace StudyGroups.Controllers
{
    public class AccountController : Controller
    {
        private StudyGroupContext db = new StudyGroupContext();

        // GET: Account/Register
        public ActionResult Register()
        {
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                // check if the email already exists
                if (db.Users.Any(u => u.Email == user.Email))
                {
                    ModelState.AddModelError("Email", "Email already registered.");
                    return View(user);
                }

                //set default role to user
                user.Role = "User";
                user.JoinedDate = DateTime.Now;
                db.Users.Add(user);
                db.SaveChanges();

                // auto login after registration
                Session["UserID"] = user.UserID;
                Session["Username"] = user.FirstName + " " + user.LastName;
                Session["UserRole"] = user.Role;

                return RedirectToAction("Index", "Home");
            }

            return View(user);
        }

        // GET: Account/Login
        public ActionResult Login()
        {
            return View();
        }

        //POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login (string email, string password)
        {
            //extract the user's credentials from the db
            var user = db.Users.FirstOrDefault(u => u.Email == email && u.Password == password);

            if (user != null)
            {
                // store the users info in session
                Session["UserID"] = user.UserID;
                Session["Username"] = user.FirstName + " " + user.LastName;
                Session["UserRole"] = user.Role;

                // redriect both admin and user to home page
                return RedirectToAction("Index", "Home");

            }

            ModelState.AddModelError("", "Invalid email or password.");
            return View();
        }

        // GET: Account/Logout
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
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