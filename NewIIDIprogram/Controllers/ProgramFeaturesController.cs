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
    public class ProgramFeaturesController : Controller
    {
        private imiMappingDbEntities db = new imiMappingDbEntities();

        // GET: ProgramFeatures
        public ActionResult Index(int? clientId)
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            string role = Session["Role"].ToString();

            var query = db.ProgramFeatures.AsQueryable();

            // ✅ FILTER BY CLIENT
            if (clientId.HasValue)
            {
                query = query.Where(x => x.UserId == clientId.Value);
                ViewBag.ClientId = clientId.Value;
            }

            // ✅ ROLE FILTER
            if (role != "Admin")
            {
                query = query.Where(x => x.UserId == userId);
            }

            return View(query.ToList());
        }



        // GET: ProgramFeatures/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProgramFeature programFeature = db.ProgramFeatures.Find(id);
            if (programFeature == null)
            {
                return HttpNotFound();
            }
            return View(programFeature);
        }

        // GET: ProgramFeatures/Create
        public ActionResult Create(int clientId)
        {
            ViewBag.ClientId = clientId;
            return View();
        }


        // POST: ProgramFeatures/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
    [Bind(Include = "FeatureId,FeatureTitle,FeatureDescription")] ProgramFeature programFeature,
    int clientId)
        {
            if (!ModelState.IsValid)
                return View(programFeature);

            string role = Session["Role"].ToString();

            programFeature.UserId = role == "Admin"
                ? clientId
                : Convert.ToInt32(Session["UserId"]);

            db.ProgramFeatures.Add(programFeature);
            db.SaveChanges();

            // 🔥 RETURN WITH clientId
            return RedirectToAction("Index", new { clientId = clientId });
        }


        // GET: ProgramFeatures/Edit/5
        public ActionResult Edit(int? id, int? clientId)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            ProgramFeature programFeature = db.ProgramFeatures.Find(id);
            if (programFeature == null)
                return HttpNotFound();

            // keep client context
            ViewBag.ClientId = clientId ?? programFeature.UserId;

            return View(programFeature);
        }


        // POST: ProgramFeatures/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
    [Bind(Include = "FeatureId,FeatureTitle,FeatureDescription")] ProgramFeature programFeature,
    int clientId)
        {
            if (!ModelState.IsValid)
                return View(programFeature);

            string role = Session["Role"].ToString();

            // VERY IMPORTANT
            programFeature.UserId = role == "Admin"
                ? clientId
                : Convert.ToInt32(Session["UserId"]);

            db.Entry(programFeature).State = EntityState.Modified;
            db.SaveChanges();

            // 🔥 Redirect WITH clientId
            return RedirectToAction("Index", new { clientId = clientId });
        }

        // GET: ProgramFeatures/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProgramFeature programFeature = db.ProgramFeatures.Find(id);
            if (programFeature == null)
            {
                return HttpNotFound();
            }
            return View(programFeature);
        }

        // POST: ProgramFeatures/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProgramFeature programFeature = db.ProgramFeatures.Find(id);
            db.ProgramFeatures.Remove(programFeature);
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
