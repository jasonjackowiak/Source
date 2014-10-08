using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using UI.Web.Models;

namespace UI.Web.Controllers
{
        [Authorize]
    public class CustomerController : Controller
    {
            public CustomerController()
        {
        }

        public ActionResult SetupProfile()
        {
            ViewBag.Message = "Setup your organisation profile.";
            return View();
        }

            [Authorize]
        public async Task<ActionResult> GetCustomer(CustomerViewModel model, string returnUrl)
        {
            //if (ModelState.IsValid)
            //{
            //    var customer = new  { UserName = model.UserName };
            //    var result = await UserManager.CreateAsync(user, model.Password);
            //    if (result.Succeeded)
            //    {
            //        //Link to Customer table

            //        await SignInAsync(user, isPersistent: false);
            //        return RedirectToAction("Profile", "Home");
            //    }
            //    else
            //    {
            //        AddErrors(result);
            //    }
            //}

            // If we got this far, something failed, redisplay form
            return View(model);
        }
    }
}