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
    public class GetInTouchesController : Controller
    {
        private imiMappingDbEntities db = new imiMappingDbEntities();

        // GET: GetInTouches
        public ActionResult Index()
        {
            if (Session["Role"] == null)
                return RedirectToAction("Login", "LoginUsers");

            if (Session["Role"].ToString() == "Admin")
            {
                return View(db.GetInTouches.ToList());
            }

            int clientId = Convert.ToInt32(Session["ClientId"]);
            var data = db.GetInTouches
                         .Where(x => x.ClientId == clientId)
                         .ToList();

            return View(data);
        }




        // GET: GetInTouches/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GetInTouch getInTouch = db.GetInTouches.Find(id);
            if (getInTouch == null)
            {
                return HttpNotFound();
            }
            return View(getInTouch);
        }

        // GET: GetInTouches/Create
        public ActionResult Create(int clientId)
        {
            ViewBag.ClientId = clientId;
            return View();
        }


        // POST: GetInTouches/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
    [Bind(Include = "Id,Name,Email,Mobile,Message")] GetInTouch getInTouch,
    int clientId)
        {
            if (!ModelState.IsValid)
                return View(getInTouch);

            // 🔥 THIS IS CRITICAL

            getInTouch.UserId = Convert.ToInt32(Session["UserId"]);
            getInTouch.ClientId = Convert.ToInt32(Session["ClientId"]); // ✅ SAFE


            db.GetInTouches.Add(getInTouch);
            db.SaveChanges();

            return RedirectToAction("Index", new { clientId = clientId });
        }




        // GET: GetInTouches/Edit/5
        public ActionResult Edit(int? id, int? clientId)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            GetInTouch getInTouch = db.GetInTouches.Find(id);
            if (getInTouch == null)
                return HttpNotFound();

            ViewBag.ClientId = getInTouch.ClientId;

            return View(getInTouch);
        }

        // POST: GetInTouches/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
     [Bind(Include = "Id,Name,Email,Mobile,Message")] GetInTouch getInTouch,
     int clientId)
        {
            if (!ModelState.IsValid)
                return View(getInTouch);

            getInTouch.ClientId = clientId;                 // ✅ REQUIRED
            getInTouch.UserId = Convert.ToInt32(Session["UserId"]);

            db.Entry(getInTouch).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index", new { clientId = clientId });
        }




        // GET: GetInTouches/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GetInTouch getInTouch = db.GetInTouches.Find(id);
            if (getInTouch == null)
            {
                return HttpNotFound();
            }
            return View(getInTouch);
        }

        // POST: GetInTouches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            GetInTouch getInTouch = db.GetInTouches.Find(id);
            db.GetInTouches.Remove(getInTouch);
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
