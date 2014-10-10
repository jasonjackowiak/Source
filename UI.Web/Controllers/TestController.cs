using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using UI.Web.Models.Domain;
using Project1;
using System.Threading.Tasks;

namespace UI.Web.Controllers
{
    
    [Authorize]
    public class TestController : Controller
    {

        //Test connecting to model
        EFCustomerCollection efCustomerCollection = new EFCustomerCollection();
        Customer customer = new Customer();


        public ActionResult CreateCustomer()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCustomer(Customer model)
        {
            //if (ModelState.IsValid)
            //{
                var c = new Customer() { Name = model.Name };
                //if (!c.Name.Equals(null))
                //{
                    //Link to Customer table
                    return RedirectToAction("ViewCustomer", "Test");
                //}

            //}
        }







        //
        // GET: /Test/
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Test/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Test/Create
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Test/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Test/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Test/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Test/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Test/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
