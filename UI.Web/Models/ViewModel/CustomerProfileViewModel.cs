using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UI.Web.Models.ViewModel
{

    public class CustomerProfileViewModel
    {
        public IEnumerable<Customer> Customers { get; set; }
        public IEnumerable<Customer> CurrentUserCustomers { get; set; }
        public IEnumerable<AspNetUser> CurrentUser { get; set; }
        public IEnumerable<Project> Projects { get; set; }
    }

}