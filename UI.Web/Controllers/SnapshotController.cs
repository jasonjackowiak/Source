using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using UI.Web.Models;

namespace UI.Web.Controllers
{
    public class SnapshotController : Controller
    {
        private FAASEntities1 db = new FAASEntities1();

        // GET: Snapshot
        public ActionResult Index()
        {
            var snapshots = db.Snapshots.Include(s => s.Project);
            return View(snapshots.ToList());
        }

        // GET: Snapshot/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Snapshot snapshot = db.Snapshots.Find(id);
            if (snapshot == null)
            {
                return HttpNotFound();
            }
            return View(snapshot);
        }

        // GET: Snapshot/Create
        public ActionResult Create()
        {
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name");
            return View();
        }

        // POST: Snapshot/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ProjectId,Status,DateTimeStamp")] Snapshot snapshot)
        {
            if (ModelState.IsValid)
            {
                db.Snapshots.Add(snapshot);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", snapshot.ProjectId);
            return View(snapshot);
        }

        // GET: Snapshot/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Snapshot snapshot = db.Snapshots.Find(id);
            if (snapshot == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", snapshot.ProjectId);
            return View(snapshot);
        }

        // POST: Snapshot/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ProjectId,Status,DateTimeStamp")] Snapshot snapshot)
        {
            if (ModelState.IsValid)
            {
                db.Entry(snapshot).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", snapshot.ProjectId);
            return View(snapshot);
        }

        // GET: Snapshot/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Snapshot snapshot = db.Snapshots.Find(id);
            if (snapshot == null)
            {
                return HttpNotFound();
            }
            return View(snapshot);
        }

        // POST: Snapshot/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Snapshot snapshot = db.Snapshots.Find(id);
            db.Snapshots.Remove(snapshot);
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
