using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Web.Models.Domain.Entity;

namespace UI.Web
{

        public interface ICustomer
        {
            IQueryable<CustomerModel> Abouts { get; }

            void SaveAbout(CustomerModel customer);

            CustomerModel DeleteAbout(int id);
        }

}
