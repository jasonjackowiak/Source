﻿using System;
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
    [Authorize]
    public class CustomerController : Controller
    {
        private FAASEntities1 db = new FAASEntities1();

        [HttpGet]
        public ActionResult Profile()
        {
            ViewBag.Message = "Your profile page.";
            return View();
        }

        //Used to maintain and build state
        [HttpPost]
        public ActionResult Profile(CustomerViewModel model)
        {
            ViewBag.Message = "Your profile page.";
            return View(model);
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
        public PartialViewResult RegisteredCustomerListPartial()
        {
            CustomerViewModel model = new CustomerViewModel();

            //Get the logged-in user record
            AspNetUser user = db.AspNetUsers
                .FirstOrDefault(f => f.UserName == User.Identity.Name);

            if (user != null)
            {
                List<int> userCustomerIds = db.AspNetUserCustomers
                .Where(x => x.UserId == user.Id)
                .Select(g => g.CustomerId).ToList();

                //construct the model for the view
                    model.CurrentUserRegisteredCustomers = db.Customers
                    .Where(x => userCustomerIds.Contains(x.Id));
            }

            return PartialView(model);
        }

        /// <summary>
        /// Display all existing customers
        /// </summary>
        /// <returns></returns>
        public PartialViewResult UnregisteredCustomerListPartial()
        {
            CustomerViewModel model = new CustomerViewModel();

            //Get the logged-in user record
            AspNetUser user = db.AspNetUsers
                .FirstOrDefault(f => f.UserName == User.Identity.Name);

            if (user != null)
            {
                List<int> userCustomerIds = db.AspNetUserCustomers
                .Where(x => x.UserId != user.Id)
                .Select(g => g.CustomerId).ToList();

                //construct the model for the view
                model.CurrentUserRegisteredCustomers = db.Customers
                .Where(x => userCustomerIds.Contains(x.Id));
            }

            return PartialView(model);
        }

        public PartialViewResult AccountCustomersPartial()
        {
            CustomerViewModel model = new CustomerViewModel();

            //Get the logged-in user record
            AspNetUser user = db.AspNetUsers
                .FirstOrDefault(f => f.UserName == User.Identity.Name);

            if (user != null)
            {
                List<int> userCustomerIds = db.AspNetUserCustomers
                .Where(x => x.UserId == user.Id)
                .Select(g => g.CustomerId).ToList();

                //construct the model for the view
                model.CurrentUserRegisteredCustomers = db.Customers
                .Where(x => userCustomerIds.Contains(x.Id));
            }

            return PartialView(model);
        }

        /// <summary>
        /// Display projects for selected customer
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CustomerProjectsPartial(int? customerId)
        {
            CustomerViewModel model = new CustomerViewModel();

            if (customerId != null)
            {
                List<int> projectIds = db.Projects
                .Where(x => x.CustomerId == customerId)
                .Select(g => g.Id).ToList();

                //construct the model for the view
                model.CustomerProjects = db.Projects
                .Where(x => projectIds.Contains(x.Id));

                ViewBag.CustomerId = customerId;
            }

            return View("Profile", model);
        }
  
    }
}
