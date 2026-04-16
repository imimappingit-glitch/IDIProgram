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
    public class ProgramsController : Controller
    {
        private imiMappingDbEntities db = new imiMappingDbEntities();

        // GET: Programs
        public ActionResult Index()
        {
            if (Session["UserId"] == null || Session["Role"] == null || Session["ValidOtp"] == null)
                return RedirectToAction("Login", "LoginUsers");

            int userId = Convert.ToInt32(Session["UserId"]);
            string role = Session["Role"].ToString();

            if (role == "Admin")
            {
                return View(db.Programs.ToList());
            }

            var programs = db.Programs
                    .Where(p => p.UserId == userId)
                     .ToList();

            return View(programs);

        }

        // GET: Programs/ClientsInProgram/5
        public ActionResult ClientsInProgram(int programId)
        {
            var program = db.Programs.Find(programId);
            if (program == null)
                return HttpNotFound();

            // Get all clients assigned to this program
            var clients = (from m in db.ClientProgramMappings
                           join c in db.ClientInformations
                           on m.ClientId equals c.ClientId
                           where m.ProgramId == programId
                           select c).ToList();

            ViewBag.ProgramName = program.ProgramName;
            return View(clients);
        }
        // GET: 
        public ActionResult DeleteClientFromProgram(int clientId, int programId)
        {
            var mapping = db.ClientProgramMappings
                            .FirstOrDefault(m => m.ClientId == clientId && m.ProgramId == programId);
            if (mapping == null)
                return HttpNotFound();

            db.ClientProgramMappings.Remove(mapping);
            db.SaveChanges();

            // Redirect back to the ClientsInProgram page
            return RedirectToAction("ClientsInProgram", new { programId = programId });
        }


        public ActionResult AddProgram(int clientId)
        {
            if (clientId == 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "ClientId missing");

            ViewBag.ClientId = clientId;
            ViewBag.ProgramId = new SelectList(db.Programs, "ProgramId", "ProgramName");

            return View();
        }



        [HttpPost]
        public ActionResult AddProgram(int clientId, int ProgramId)
        {
            if (clientId == 0 || ProgramId == 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            bool alreadyExists = db.ClientProgramMappings
                .Any(x => x.ClientId == clientId && x.ProgramId == ProgramId);

            if (!alreadyExists)
            {
                var mapping = new ClientProgramMapping
                {
                    ClientId = clientId,
                    ProgramId = ProgramId
                };

                db.ClientProgramMappings.Add(mapping);
                db.SaveChanges();
            }


            return RedirectToAction("ClientsInProgram", "Programs", new { programId = ProgramId });
        }

        // GET: Programs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var program = db.Programs.Find(id);
            if (program == null)
                return HttpNotFound();

            var clients = (from m in db.ClientProgramMappings
                           join c in db.ClientInformations
                           on m.ClientId equals c.ClientId
                           where m.ProgramId == id
                           select c).ToList();

            ViewBag.Clients = clients;

            return View(program);
        }



        // GET: Programs/Create
        public ActionResult Create()
        {
            if (Session["Role"]?.ToString() != "Admin")
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            ViewBag.Users = db.LoginUsers
                              .Where(u => u.RoleId == 1)
                              .ToList();

            return View();
        }



        // POST: Programs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Program program)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "LoginUsers");

            if (Session["Role"]?.ToString() != "Admin")
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            // ✅ AUTO ASSIGN logged-in Admin
            program.UserId = Convert.ToInt32(Session["UserId"]);

            if (!ModelState.IsValid)
                return View(program);

            db.Programs.Add(program);
            db.SaveChanges();

            return RedirectToAction("Index");
        }





        // GET: Programs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["UserId"] == null || Session["Role"] == null)
                return RedirectToAction("Login", "LoginUsers");

            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            int userId = Convert.ToInt32(Session["UserId"]);
            string role = Session["Role"].ToString();

            Program program = role == "Admin"
                ? db.Programs.Find(id)
                : db.Programs.FirstOrDefault(p => p.ProgramId == id && p.UserId == userId);

            if (program == null)
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            // 🔥 LOAD USERS ONLY FOR ADMIN
            if (role == "Admin")
            {
                ViewBag.Users = db.LoginUsers
                                  .Where(u => u.RoleId == 1)
                                  .ToList();
            }

            return View(program);
        }


        // POST: Programs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Program program)
        {
            if (Session["UserId"] == null || Session["RoleId"] == null)
                return RedirectToAction("Login", "LoginUsers");

            int userId = Convert.ToInt32(Session["UserId"]);
            string role = Session["Role"].ToString();

            var existing = role == "Admin"
                ? db.Programs.Find(program.ProgramId)
                : db.Programs.FirstOrDefault(p => p.ProgramId == program.ProgramId && p.UserId == userId);

            if (existing == null)
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            existing.ProgramName = program.ProgramName;
            existing.Description = program.Description;

            // 🔥 ADMIN CAN CHANGE USER
            if (role == "Admin")
            {
                existing.UserId = program.UserId;
            }

            db.SaveChanges();
            return RedirectToAction("Index");
        }



        // GET: Programs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Program program = db.Programs.Find(id);
            if (program == null)
            {
                return HttpNotFound();
            }
            return View(program);
        }

        // POST: Programs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var program = db.Programs.Find(id);
            if (program == null)
                return HttpNotFound();

            // 🔴 STEP 1: delete ProgramStatus
            var statuses = db.ProgramStatus.Where(s => s.ProgramId == id).ToList();
            db.ProgramStatus.RemoveRange(statuses);

            // 🔴 STEP 2: delete ProgramSteps (if exists)
            var steps = db.ProgramSteps.Where(s => s.ProgramId == id).ToList();
            db.ProgramSteps.RemoveRange(steps);

            // 🔴 STEP 3: delete Program
            db.Programs.Remove(program);

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
