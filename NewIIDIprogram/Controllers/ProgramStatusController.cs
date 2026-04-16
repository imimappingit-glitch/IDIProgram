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
    public class ProgramStatusController : Controller
    {
        private imiMappingDbEntities db = new imiMappingDbEntities();

        // GET: ProgramStatus
        public ActionResult Index(int? programId, int? stepId, int? clientId)
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            string role = Session["Role"].ToString();

            // Pick default program and step if null
            if (!programId.HasValue || !stepId.HasValue)
            {
                var firstProgram = db.Programs.FirstOrDefault();
                var firstStep = db.ProgramSteps.FirstOrDefault();

                programId = programId ?? firstProgram?.ProgramId ?? 1;
                stepId = stepId ?? firstStep?.StepId ?? 1;
            }

            var query = db.ProgramStatus
                          .Where(x => x.ProgramId == programId.Value &&
                                      x.StepId == stepId.Value);

            if (role != "Admin")
            {
                query = query.Where(x => x.UserId == userId);
            }

            ViewBag.ProgramId = programId.Value;
            ViewBag.StepId = stepId.Value;
            ViewBag.ClientId = clientId;

            return View(query.ToList());
        }

        // GET: ProgramStatus/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            ProgramStatu programStatu = db.ProgramStatus.Find(id);
            if (programStatu == null)
                return HttpNotFound();

            return View(programStatu);
        }

        // GET: ProgramStatus/Create
        public ActionResult Create(int? programId, int? stepId, int? clientId)
        {
            var firstProgram = db.Programs.FirstOrDefault();
            var firstStep = db.ProgramSteps.FirstOrDefault();

            int defaultProgramId = programId ?? firstProgram?.ProgramId ?? 1;
            int defaultStepId = stepId ?? firstStep?.StepId ?? 1;

            ViewBag.ProgramId = defaultProgramId;
            ViewBag.StepId = defaultStepId;
            ViewBag.ClientId = clientId ?? Convert.ToInt32(Session["UserId"]);

            return View();
        }

        // POST: ProgramStatus/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProgramStatu programStatu, int? programId, int? stepId, int? clientId)
        {
            if (!ModelState.IsValid)
                return View(programStatu);

            string role = Session["Role"].ToString();

            programStatu.UserId = role == "Admin"
                ? (clientId ?? Convert.ToInt32(Session["UserId"]))
                : Convert.ToInt32(Session["UserId"]);

            var firstProgram = db.Programs.FirstOrDefault();
            var firstStep = db.ProgramSteps.FirstOrDefault();

            programStatu.ProgramId = programId ?? firstProgram?.ProgramId ?? 1;
            programStatu.StepId = stepId ?? firstStep?.StepId ?? 1;
            programStatu.CreatedAt = DateTime.Now;

            db.ProgramStatus.Add(programStatu);
            db.SaveChanges();

            return RedirectToAction("Index", new
            {
                programId = programStatu.ProgramId,
                stepId = programStatu.StepId,
                clientId
            });
        }

        // GET: ProgramStatus/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            ProgramStatu programStatu = db.ProgramStatus.Find(id);
            if (programStatu == null)
                return HttpNotFound();

            return View(programStatu);
        }

        // POST: ProgramStatus/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
            [Bind(Include = "ProgramStatusId,ClassDate,ClassTime,Navigator,ClassDescription,CreatedAt")]
            ProgramStatu programStatu,
            int clientId)
        {
            if (!ModelState.IsValid)
                return View(programStatu);

            string role = Session["Role"].ToString();

            programStatu.UserId = role == "Admin"
                ? clientId
                : Convert.ToInt32(Session["UserId"]);

            db.Entry(programStatu).State = EntityState.Modified;
            db.SaveChanges();

            // Redirect safely with programId and stepId
            return RedirectToAction("Index", new
            {
                programId = programStatu.ProgramId,
                stepId = programStatu.StepId,
                clientId
            });
        }

        // GET: ProgramStatus/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            ProgramStatu programStatu = db.ProgramStatus.Find(id);
            if (programStatu == null)
                return HttpNotFound();

            return View(programStatu);
        }

        // POST: ProgramStatus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProgramStatu programStatu = db.ProgramStatus.Find(id);
            db.ProgramStatus.Remove(programStatu);
            db.SaveChanges();

            // Redirect to Index with safe defaults
            return RedirectToAction("Index", new
            {
                programId = programStatu.ProgramId,
                stepId = programStatu.StepId,
                clientId = programStatu.UserId
            });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }
    }
}
