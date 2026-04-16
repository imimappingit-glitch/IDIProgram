using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using NewIIDIprogram.Models;

namespace NewIIDIprogram.Controllers
{
    public class LoginUsersController : Controller
    {
        private imiMappingDbEntities db = new imiMappingDbEntities();

        // GET: LoginUsers
        public ActionResult Index()
        {
            string role = Session["Role"]?.ToString();

            if (role == "Admin")
            {
                // Admin sees all users
                return View(db.LoginUsers.ToList());
            }
            else
            {
                // Normal user sees only their own data
                string currentUserEmail = Session["Email"]?.ToString();
                var userData = db.LoginUsers
                                 .Where(u => u.Email == currentUserEmail)
                                 .ToList();
                return View(userData);
            }
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        // POST: /LoginUsers/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginUser loginObj)
        {
            var user = db.LoginUsers.FirstOrDefault(u =>
                u.Email == loginObj.Email && u.Password == loginObj.Password);

            if (user != null)
            {
                // Save session
                Session["UserId"] = user.UserId;
                Session["Email"] = user.Email;
                Session["RoleId"] = user.RoleId;
                Session["UserName"] = user.FullName;

                if (user.RoleId == 1)
                {
                    Session["Role"] = "Admin";
                }
                else if (user.RoleId == 2)
                {
                    Session["Role"] = "User";
                }
                else
                {
                    Session["Role"] = "Unknown";
                }


                // Generate OTP (for demo, we use "1111")
                Session["otp"] = "1111";

                return RedirectToAction("ValidateLogin");
            }


            ViewData["Message"] = "Invalid Email or Password!";
            return View();
        }

        // GET: /LoginUsers/ValidateLogin
        [HttpGet]
        public ActionResult ValidateLogin()
        {
            if (Session["Email"] == null)
                return RedirectToAction("Login");

            return View();
        }

        // POST: /LoginUsers/ValidateLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ValidateLogin(LoginUser loginObj)
        {
            if (Session["Email"] == null)
                return RedirectToAction("Login");

            if (loginObj.Password == Session["otp"]?.ToString() || loginObj.Password == "1111")
            {
                Session["ValidOtp"] = "true";

                // Redirect based on role
                if (Session["Role"].ToString() == "Admin")
                    return RedirectToAction("Index", "Programs");

                if (Session["Role"].ToString() == "User")
                    return RedirectToAction("Index", "Programs");



                return RedirectToAction("Index", "LoginUsers");
            }

            ViewData["Message"] = "Invalid OTP!";
            return View();
        }

        // POST: /LoginUsers/Logout
        [HttpPost]
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }

        // GET: LoginUsers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LoginUser loginUser = db.LoginUsers.Find(id);
            if (loginUser == null)
            {
                return HttpNotFound();
            }
            return View(loginUser);
        }

        // GET: LoginUsers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LoginUsers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserId,FullName,Email,Password,RoleId,CreatedDate")] LoginUser loginUser)
        {
            if (ModelState.IsValid)
            {
                db.LoginUsers.Add(loginUser);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(loginUser);
        }

        // GET: LoginUsers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LoginUser loginUser = db.LoginUsers.Find(id);
            if (loginUser == null)
            {
                return HttpNotFound();
            }
            return View(loginUser);
        }

        // POST: LoginUsers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserId,FullName,Email,Password,RoleId,CreatedDate")] LoginUser loginUser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(loginUser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(loginUser);
        }

        // GET: LoginUsers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LoginUser loginUser = db.LoginUsers.Find(id);
            if (loginUser == null)
            {
                return HttpNotFound();
            }
            return View(loginUser);
        }

        // POST: LoginUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LoginUser loginUser = db.LoginUsers.Find(id);
            db.LoginUsers.Remove(loginUser);
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
