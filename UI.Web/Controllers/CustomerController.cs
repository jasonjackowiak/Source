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
    public class CustomerController : Controller
    {
        private FAASEntities1 db = new FAASEntities1();



        // GET: /Customer/
        public ActionResult Index()
        {
            return View(db.Customers.ToList());
        }

        // GET: /Customer/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // GET: /Customer/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Customer/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                db.Customers.Add(customer);
                db.SaveChanges();

                //Get the logged-in user record
                AspNetUser record = db.AspNetUsers
                    .FirstOrDefault(x => x.UserName == User.Identity.Name);

                

                //AspNetUserCustomer userCustomer = new AspNetUserCustomer(record.Id, customer.Id);

                //db.AspNetUserCustomers.Add(userCustomer);

                if (record == null)
                {
                    foreach (AspNetUser user in db.AspNetUsers)
                    {
                        if (user.UserName == User.Identity.Name)
                            record = user;
                    }
                }

                //record.CustomerId = customer.Id;
                db.AspNetUsers.Add(record);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(customer);
        }

        // GET: /Customer/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: /Customer/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(customer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(customer);
        }

        // GET: /Customer/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: /Customer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Customer customer = db.Customers.Find(id);
            db.Customers.Remove(customer);
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

        //Manually added

        /// <summary>
        /// Display customers for logged in user
        /// </summary>
        /// <returns></returns>
        public PartialViewResult CustomerNamePartial()
        {
            var id1 = db.AspNetUsers
                      .Where(x => x.UserName == User.Identity.Name)
                      .FirstOrDefault();

            //var customers = db.Customers
            //    .AsEnumerable(x => x.AspNetUsers);

            try
            {
                //IEnumerable<Customer> customer = db.Customers.Find(id1.Customers);
                Customer customer = db.Customers.Find(id1);
                return PartialView(customer);
            }
            catch
            {
                return PartialView();
            }
        }

        /// <summary>
        /// Display all existing customers
        /// </summary>
        /// <returns></returns>
        public PartialViewResult CustomerListPartial()
        {
            SelectList customerList = new SelectList(db.Customers.ToList(), "Name", "Name");
            return PartialView(customerList);
        }

    }
}
