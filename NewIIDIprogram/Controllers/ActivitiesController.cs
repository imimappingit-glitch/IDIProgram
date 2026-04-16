using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
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

            if (stepId.HasValue)
            {
                activities = activities.Where(a => a.StepId == stepId.Value);
                ViewBag.CurrentStepId = stepId.Value;
            }

            return View(activities.ToList());
        }

        // GET: Activities/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Activity activity = db.Activities
                .Include(a => a.ProgramStep)
                .FirstOrDefault(a => a.ActivityId == id);

            if (activity == null)
                return HttpNotFound();

            return View(activity);
        }

        // GET: Activities/Create
        public ActionResult Create(int? stepId)
        {
            if (stepId == null)
                return RedirectToAction("Index", "Programs");

            var step = db.ProgramSteps
                .Where(s => s.StepId == stepId.Value)
                .Select(s => new { s.StepId, s.StepTitle })
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ActivityId,StepId,ActivityName,DoneBy,ActivityDate,Target,Benefit,Duration,Pricing")] Activity activity)
        {
            if (ModelState.IsValid)
            {
                db.Activities.Add(activity);
                db.SaveChanges();
                return RedirectToAction("Index", new { stepId = activity.StepId });
            }

            return View(activity);
        }

        // GET: Activities/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Activity activity = db.Activities.Find(id);

            if (activity == null)
                return HttpNotFound();

            ViewBag.StepId = new SelectList(db.ProgramSteps, "StepId", "StepTitle", activity.StepId);
            return View(activity);
        }

        // POST: Activities/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ActivityId,StepId,ActivityName,DoneBy,ActivityDate,Target,Benefit,Duration,Pricing")] Activity activity)
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
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Activity activity = db.Activities.Find(id);

            if (activity == null)
                return HttpNotFound();

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
                db.Dispose();

            base.Dispose(disposing);
        }
    }
}