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
    public class ProgramStepsController : Controller
    {
        private imiMappingDbEntities db = new imiMappingDbEntities();

        // GET: ProgramSteps
        public ActionResult Index(int? programId)
        {
            if (programId == null)
                return RedirectToAction("Index", "Programs");

            if (Session["UserId"] == null || Session["Role"] == null)
                return RedirectToAction("Login", "LoginUsers");

            int userId = Convert.ToInt32(Session["UserId"]);
            string role = Session["Role"].ToString();

            var program = db.Programs.Find(programId);
            if (program == null)
                return HttpNotFound();

            // 🔐 ownership check
            if (role != "Admin" && program.UserId != userId)
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            var steps = db.ProgramSteps
                .Include(s => s.Program)
                .Where(s => s.ProgramId == programId)
                .OrderBy(s => s.MasterStepId)
                .ToList();

            ViewBag.ProgramName = program.ProgramName;
            return View(steps);
        }

        public ActionResult Dashboard(int programId)
        {
            // Step 1: check login
            if (Session["UserId"] == null || Session["Role"] == null)
                return RedirectToAction("Login", "LoginUsers");

            // Step 2: read session
            int userId = Convert.ToInt32(Session["UserId"]);
            string role = Session["Role"].ToString();

            // Step 3: load program
            var program = db.Programs.Find(programId);
            if (program == null)
                return HttpNotFound();

            // Step 4: ownership check
            if (role != "Admin" && program.UserId != userId)
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            // Step 5: load steps
            var steps = db.ProgramSteps
                .Include(x => x.MasterStep)
                .Include(x => x.Program)
                .Include(x => x.Activities)
                .Where(x => x.ProgramId == programId)
                .OrderBy(x => x.MasterStepId)
                .ToList();

            // 🔥 Load clients from mapping table
            var clients = (from m in db.ClientProgramMappings
                           join c in db.ClientInformations
                           on m.ClientId equals c.ClientId
                           where m.ProgramId == programId
                           select c).ToList();
            ViewBag.Clients = clients;

            ViewBag.ProgramId = programId;
            ViewBag.ProgramName = program.ProgramName;

            return View(steps);
        }







        // GET: ProgramSteps/Details/5

        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            // Get the current step
            ProgramStep programStep = db.ProgramSteps.Find(id);
            if (programStep == null)
                return HttpNotFound();

            // Get all steps for this program ordered by StepTitle or MasterStepId
            var programSteps = db.ProgramSteps
                                 .Where(s => s.ProgramId == programStep.ProgramId)
                                 .OrderBy(s => s.MasterStepId) // or StepId / StepOrder
                                 .ToList();

            // Determine completed steps (all steps before or equal to current step)
            var completedSteps = programSteps
                                 .TakeWhile(s => s.StepId != programStep.StepId)
                                 .Select(s => programSteps.IndexOf(s) + 1) // slice numbers
                                 .ToList();

            // Include current step as completed
            completedSteps.Add(programSteps.IndexOf(programStep) + 1);

            ViewBag.ProgramSteps = programSteps;
            ViewBag.CompletedSteps = completedSteps;

            return View(programStep);
        }




        // GET: ProgramSteps/Create
        public ActionResult Create()
        {
            // Use ProgramId for the dropdown value
            ViewBag.ProgramId = new SelectList(db.Programs, "ProgramId", "ProgramName");
            ViewBag.StepTitle = new SelectList(db.MasterSteps, "StepTitle", "StepTitle"); // ✅ Use StepTitle for both value & text
            return View();
        }


        // POST: ProgramSteps/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "StepId,StepTitle,StepDescription,ProgramId")] ProgramStep programStep)
        {
            if (ModelState.IsValid)
            {
                db.ProgramSteps.Add(programStep);
                db.SaveChanges();

                return RedirectToAction("Index", new { programId = programStep.ProgramId });

            }

            // Repopulate dropdowns
            ViewBag.ProgramId = new SelectList(db.Programs, "ProgramId", "ProgramName", programStep.ProgramId);
            ViewBag.StepTitle = new SelectList(db.MasterSteps, "StepTitle", "StepTitle", programStep.StepTitle);
            return View(programStep);
        }







        // GET: ProgramSteps/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            ProgramStep programStep = db.ProgramSteps.Find(id);
            if (programStep == null)
                return HttpNotFound();

            // Populate dropdown
            ViewBag.ProgramId = new SelectList(db.Programs, "ProgramId", "ProgramName", programStep.ProgramId);
            // ✅ ADD THIS
            ViewBag.StepList = new SelectList(
                db.MasterSteps,
                "StepTitle",
                "StepTitle",
                programStep.StepTitle   // selected value
            );
            return View(programStep);


        }


        // POST: ProgramSteps/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "StepId,StepTitle,StepDescription,ProgramId")] ProgramStep model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ProgramId = new SelectList(db.Programs, "ProgramId", "ProgramName", model.ProgramId);
                ViewBag.StepList = new SelectList(db.MasterSteps, "StepTitle", "StepTitle", model.StepTitle);
                return View(model);
            }

            // ✅ Fetch existing row
            var programStep = db.ProgramSteps.Find(model.StepId);
            if (programStep == null)
                return HttpNotFound();

            // ✅ Update only what you want
            programStep.ProgramId = model.ProgramId;
            programStep.StepTitle = model.StepTitle;
            programStep.StepDescription = model.StepDescription;

            db.SaveChanges();
            return RedirectToAction("Index");
        }


        // GET: ProgramSteps/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProgramStep programStep = db.ProgramSteps.Find(id);
            if (programStep == null)
            {
                return HttpNotFound();
            }
            return View(programStep);
        }

        // POST: ProgramSteps/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProgramStep programStep = db.ProgramSteps.Find(id);
            db.ProgramSteps.Remove(programStep);
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
