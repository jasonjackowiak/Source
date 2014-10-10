using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project1;

namespace UI.Web
{

        public interface ICustomer
        {
            IQueryable<Customer> Customers { get; }

            void SaveCustomer(Customer customer);

            Customer DeleteCustomer(int id);
        }

}
