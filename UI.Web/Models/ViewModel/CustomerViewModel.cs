using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UI.Web.Models.ViewModel
{

    public class CustomerViewModel
    {
        public IEnumerable<Customer> Customers { get; set; }
        public IEnumerable<Customer> CurrentUserRegisteredCustomers { get; set; }
        public IEnumerable<Customer> CurrentUserUnregisteredCustomers { get; set; }
        public IEnumerable<Customer> SelectedCustomer { get; set; }
        public IEnumerable<AspNetUser> CurrentUser { get; set; }
        public IEnumerable<Project> Projects { get; set; }
        public IEnumerable<Project> CustomerProjects { get; set; }
        public IEnumerable<Snapshot> Snapshots { get; set; }
    }

}