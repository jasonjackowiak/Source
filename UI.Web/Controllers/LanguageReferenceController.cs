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
    public class LanguageReferenceController : Controller
    {
        private FAASEntities1 db = new FAASEntities1();

        // GET: LanguageReference
        public ActionResult Index()
        {
            return View(db.LanguageReferences.ToList());
        }

        // GET: LanguageReference/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LanguageReference languageReference = db.LanguageReferences.Find(id);
            if (languageReference == null)
            {
                return HttpNotFound();
            }
            return View(languageReference);
        }

        // GET: LanguageReference/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LanguageReference/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Variant,Version")] LanguageReference languageReference)
        {
            if (ModelState.IsValid)
            {
                db.LanguageReferences.Add(languageReference);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(languageReference);
        }

        // GET: LanguageReference/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LanguageReference languageReference = db.LanguageReferences.Find(id);
            if (languageReference == null)
            {
                return HttpNotFound();
            }
            return View(languageReference);
        }

        // POST: LanguageReference/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Variant,Version")] LanguageReference languageReference)
        {
            if (ModelState.IsValid)
            {
                db.Entry(languageReference).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(languageReference);
        }

        // GET: LanguageReference/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LanguageReference languageReference = db.LanguageReferences.Find(id);
            if (languageReference == null)
            {
                return HttpNotFound();
            }
            return View(languageReference);
        }

        // POST: LanguageReference/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LanguageReference languageReference = db.LanguageReferences.Find(id);
            db.LanguageReferences.Remove(languageReference);
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
