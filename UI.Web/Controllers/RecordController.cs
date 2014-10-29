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
    public class RecordController : Controller
    {
        private FAASEntities1 db = new FAASEntities1();

        // GET: Record
        public ActionResult Index()
        {
            var records = db.Records.Include(r => r.LanguageReference).Include(r => r.Snapshot);
            return View(records.ToList());
        }

        // GET: Record/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Record record = db.Records.Find(id);
            if (record == null)
            {
                return HttpNotFound();
            }
            return View(record);
        }

        // GET: Record/Create
        public ActionResult Create()
        {
            ViewBag.LanguageId = new SelectList(db.LanguageReferences, "Id", "Name");
            ViewBag.SnapshotId = new SelectList(db.Snapshots, "Id", "Status");
            return View();
        }

        // POST: Record/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,SnapshotId,LanguageId,InputFileName,DateTimeStamp")] Record record)
        {
            if (ModelState.IsValid)
            {
                db.Records.Add(record);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.LanguageId = new SelectList(db.LanguageReferences, "Id", "Name", record.LanguageId);
            ViewBag.SnapshotId = new SelectList(db.Snapshots, "Id", "Status", record.SnapshotId);
            return View(record);
        }

        // GET: Record/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Record record = db.Records.Find(id);
            if (record == null)
            {
                return HttpNotFound();
            }
            ViewBag.LanguageId = new SelectList(db.LanguageReferences, "Id", "Name", record.LanguageId);
            ViewBag.SnapshotId = new SelectList(db.Snapshots, "Id", "Status", record.SnapshotId);
            return View(record);
        }

        // POST: Record/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,SnapshotId,LanguageId,InputFileName,DateTimeStamp")] Record record)
        {
            if (ModelState.IsValid)
            {
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.LanguageId = new SelectList(db.LanguageReferences, "Id", "Name", record.LanguageId);
            ViewBag.SnapshotId = new SelectList(db.Snapshots, "Id", "Status", record.SnapshotId);
            return View(record);
        }

        // GET: Record/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Record record = db.Records.Find(id);
            if (record == null)
            {
                return HttpNotFound();
            }
            return View(record);
        }

        // POST: Record/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Record record = db.Records.Find(id);
            db.Records.Remove(record);
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
