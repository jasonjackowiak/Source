using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using UI.Web.Models.Domain.Entity;

namespace UI.Web.Models.Domain
{
    public class EFdbContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }
    }
}