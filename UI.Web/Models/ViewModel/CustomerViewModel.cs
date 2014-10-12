using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity.EntityFramework;
using UI.Web.Models.Domain.Entity;

namespace UI.Web.Models
{

    public class CustomerViewModel
    {
        public IEnumerable<Customer> Customers { get; set; }
    }
}