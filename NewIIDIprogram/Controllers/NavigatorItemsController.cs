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
    public class NavigatorItemsController : Controller
    {
        private imiMappingDbEntities db = new imiMappingDbEntities();

        // GET: NavigatorItems
        public ActionResult Index(int? clientId)
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            string role = Session["Role"].ToString();

            var query = db.NavigatorItems.AsQueryable();

            // Filter by selected client
            if (clientId.HasValue)
            {
                query = query.Where(x => x.UserId == clientId.Value);
                ViewBag.ClientId = clientId.Value;
            }

            // Non-admin sees only their own data
            if (role != "Admin")
            {
                query = query.Where(x => x.UserId == userId);
            }

            return View(query.ToList());
        }



        // GET: NavigatorItems/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NavigatorItem navigatorItem = db.NavigatorItems.Find(id);
            if (navigatorItem == null)
            {
                return HttpNotFound();
            }
            return View(navigatorItem);
        }

        // GET: NavigatorItems/Create
        public ActionResult Create(int clientId)
        {
            ViewBag.ClientId = clientId;
            return View();
        }


        // POST: NavigatorItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
    [Bind(Include = "ItemId,ItemTitle,ItemDescription")] NavigatorItem navigatorItem,
    int clientId)
        {
            if (!ModelState.IsValid)
                return View(navigatorItem);

            string role = Session["Role"].ToString();

            navigatorItem.UserId = role == "Admin"
                ? clientId
                : Convert.ToInt32(Session["UserId"]);

            db.NavigatorItems.Add(navigatorItem);
            db.SaveChanges();

            // Pass clientId back to Index
            return RedirectToAction("Index", new { clientId = clientId });
        }


        // GET: NavigatorItems/Edit/5
        public ActionResult Edit(int? id, int? clientId)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            NavigatorItem navigatorItem = db.NavigatorItems.Find(id);
            if (navigatorItem == null)
                return HttpNotFound();

            // keep client context
            ViewBag.ClientId = clientId ?? navigatorItem.UserId;

            return View(navigatorItem);
        }

        // POST: NavigatorItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
     [Bind(Include = "ItemId,ItemTitle,ItemDescription")] NavigatorItem navigatorItem,
     int clientId)
        {
            if (!ModelState.IsValid)
                return View(navigatorItem);

            string role = Session["Role"].ToString();

            navigatorItem.UserId = role == "Admin"
                ? clientId
                : Convert.ToInt32(Session["UserId"]);

            db.Entry(navigatorItem).State = EntityState.Modified;
            db.SaveChanges();

            // 🔥 THIS IS THE FIX
            return RedirectToAction("Index", new { clientId = clientId });
        }


        // GET: NavigatorItems/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NavigatorItem navigatorItem = db.NavigatorItems.Find(id);
            if (navigatorItem == null)
            {
                return HttpNotFound();
            }
            return View(navigatorItem);
        }

        // POST: NavigatorItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            NavigatorItem navigatorItem = db.NavigatorItems.Find(id);
            db.NavigatorItems.Remove(navigatorItem);
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
