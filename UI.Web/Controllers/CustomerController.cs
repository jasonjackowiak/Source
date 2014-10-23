using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using UI.Web.Models;
using UI.Web.Models.ViewModel;

namespace UI.Web.Controllers
{
    public class CustomerController : Controller
    {
        private FAASEntities1 db = new FAASEntities1();

        public ActionResult Profile()
        {
            ViewBag.Message = "Your profile page.";

            return View();
        }

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

                //Add record to link customer to user
                AspNetUserCustomer userCustomer = new AspNetUserCustomer();
                userCustomer.CustomerId = customer.Id;
                userCustomer.UserId = record.Id;

                db.AspNetUserCustomers.Add(userCustomer);
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
        public PartialViewResult AccountCustomersPartial()
        {
            CustomerViewModel model = new CustomerViewModel();

            //Get the logged-in user record
            AspNetUser record = db.AspNetUsers
                .FirstOrDefault(f => f.UserName == User.Identity.Name);

            //Use current userId to grab all related customers
            IEnumerable<AspNetUserCustomer> userCustomers = db.AspNetUserCustomers
                .Where(x => x.UserId == record.Id);

            if (userCustomers != null)
            {
                List<int> userCustomerIds = db.AspNetUserCustomers
                .Where(x => x.UserId == record.Id)
                .Select(g => g.CustomerId).ToList();

                //construct the model for the view
                    model.CurrentUserCustomers = db.Customers
                    .Where(x => userCustomerIds.Contains(x.Id));
            }

            return PartialView(model);
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

        /// <summary>
        /// Display selected customer's projects
        /// </summary>
        /// <returns></returns>
        public PartialViewResult CustomerProjectListPartial(int? id)
        {
            //user will select a customer from the dropdown
            //selected customer is passed here

            CustomerViewModel model = new CustomerViewModel();

            SelectList projectList = new SelectList(db.Projects
                .Where(x => x.CustomerId == id).ToList(), "Name", "Name");
            return PartialView(projectList);
        }

    }
}
