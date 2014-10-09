using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Project1;

namespace UI.Web.Models.Domain
{
    public class EFCustomerCollection : ICustomer
    {

        private FAASModel context = new FAASModel();

        public IQueryable<Customer> Customers
        {
            get { return context.Customers; }
        }

        public void SaveCustomer(Customer customer)
        {

            if (customer.Id == 0)
            {
                context.Customers.Add(customer);
            }
            else
            {
                Customer dbEntry = context.Customers.Find(customer.Id);
                if (dbEntry != null)
                {
                    dbEntry.Name = customer.Name;
                }
            }
            context.SaveChanges();
        }

        public Customer DeleteAbout(int id)
        {
            Customer dbEntry = context.Customers.Find(id);
            if (dbEntry != null)
            {
                context.Customers.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }
    }
}