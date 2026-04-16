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
    public class ActivitiesController : Controller
    {
        private imiMappingDbEntities db = new imiMappingDbEntities();

        // GET: Activities
        public ActionResult Index(int? stepId)
        {
            var activities = db.Activities.Include(a => a.ProgramStep);

            // Filter by StepId if passed from ProgramStep Index
            if (stepId.HasValue)
            {
                activities = activities.Where(a => a.StepId == stepId.Value);
                ViewBag.CurrentStepId = stepId.Value; // optional, for display in View
            }

            return View(activities.ToList());
        }

        // GET: Activities/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Activity activity = db.Activities.Find(id);
            if (activity == null)
            {
                return HttpNotFound();
            }
            return View(activity);
        }

        // GET: Activities/Create
        public ActionResult Create(int? stepId)
        {
            if (stepId == null)
                return RedirectToAction("Index", "Programs");

            var step = db.ProgramSteps
                .Where(s => s.StepId == stepId.Value)
                .Select(s => new
                {
                    s.StepId,
                    s.StepTitle
                })
                .FirstOrDefault();

            if (step == null)
                return HttpNotFound();

            ViewBag.StepTitle = step.StepTitle;

            return View(new Activity
            {
                StepId = step.StepId,
                ActivityDate = DateTime.Today
            });
        }


        // POST: Activities/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ActivityId,StepId,ActivityName,DoneBy,ActivityDate")] Activity activity)
        {
            if (ModelState.IsValid)
            {
                db.Activities.Add(activity);
                db.SaveChanges();
                return RedirectToAction("Index", new { stepId = activity.StepId });

            }

            ViewBag.StepId = new SelectList(db.ProgramSteps, "StepId", "StepTitle", activity.StepId);
            return View(activity);
        }

        // GET: Activities/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Activity activity = db.Activities.Find(id);
            if (activity == null)
            {
                return HttpNotFound();
            }
            ViewBag.StepId = new SelectList(db.ProgramSteps, "StepId", "StepTitle", activity.StepId);
            return View(activity);
        }

        // POST: Activities/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ActivityId,StepId,ActivityName,DoneBy,ActivityDate")] Activity activity)
        {
            if (ModelState.IsValid)
            {
                db.Entry(activity).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { stepId = activity.StepId });

            }
            ViewBag.StepId = new SelectList(db.ProgramSteps, "StepId", "StepTitle", activity.StepId);
            return View(activity);
        }

        // GET: Activities/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Activity activity = db.Activities.Find(id);
            if (activity == null)
            {
                return HttpNotFound();
            }
            return View(activity);
        }

        // POST: Activities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Activity activity = db.Activities.Find(id);
            db.Activities.Remove(activity);
            db.SaveChanges();
            return RedirectToAction("Index", new { stepId = activity.StepId });

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
